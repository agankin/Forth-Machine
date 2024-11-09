using System.Collections.Immutable;

namespace ForthMachine;

public record BeginLoopScope(
    ImmutableList<string> LoopWords,
    int NestedLoops
) : SyntacticScope()
{
    public static readonly BeginLoopScope Empty = new(
        ImmutableList<string>.Empty,
        NestedLoops: 0
    );

    public BeginLoopScope AddWord(string word) => this with { LoopWords = LoopWords.Add(word) };

    public BeginLoopScope IncNestedLoops() => this with { NestedLoops = NestedLoops + 1 };

    public BeginLoopScope DecNestedLoops() => NestedLoops > 0
        ? this with { NestedLoops = NestedLoops + 1 }
        : throw new Exception("NestedLoops value cannot be less zero.");
}