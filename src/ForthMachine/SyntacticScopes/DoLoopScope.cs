using System.Collections.Immutable;

namespace ForthMachine;

public record DoLoopScope(
    ImmutableList<string> LoopWords,
    int NestedLoops,
    int Index,
    int Limit
) : SyntacticScope()
{
    public static DoLoopScope Create(int index, int limit) => new(
        ImmutableList<string>.Empty,
        NestedLoops: 0,
        Index: index,
        Limit: limit
    );

    public DoLoopScope IncIndex() => ReachedLimit()
        ? throw new Exception("Index out of limit.")
        : this with { Index = Index + 1 };

    public bool ReachedLimit() => Index >= Limit - 1;

    public DoLoopScope AddWord(string word) => this with { LoopWords = LoopWords.Add(word) };

    public DoLoopScope IncNestedLoops() => this with { NestedLoops = NestedLoops + 1 };

    public DoLoopScope DecNestedLoops() => NestedLoops > 0
        ? this with { NestedLoops = NestedLoops + 1 }
        : throw new Exception("NestedLoops value cannot be less zero.");
}