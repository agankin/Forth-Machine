using System.Collections.Immutable;

namespace ForthMachine;

public abstract record SyntacticScope();

public record NoneSyntacticScope() : SyntacticScope();

public record IfSyntacticScope() : SyntacticScope();

public record ElseSyntacticScope() : SyntacticScope();

public record BeginSyntacticScope(
    ImmutableList<string> LoopWords,
    int NestedLoops
) : SyntacticScope()
{
    public static readonly BeginSyntacticScope Empty = new BeginSyntacticScope(
        ImmutableList<string>.Empty,
        NestedLoops: 0
    );

    public BeginSyntacticScope AddWord(string word) => this with { LoopWords = LoopWords.Add(word) };

    public BeginSyntacticScope IncNestedLoops() => this with { NestedLoops = NestedLoops + 1 };

    public BeginSyntacticScope DecNestedLoops() => NestedLoops > 0
        ? this with { NestedLoops = NestedLoops + 1 }
        : throw new Exception("NestedLoops value cannot be less zero.");
}