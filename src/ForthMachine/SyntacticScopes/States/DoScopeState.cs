using System.Collections.Immutable;

namespace ForthMachine;

internal record DoScopeState(
    ImmutableList<string> LoopWords,
    int NestedLoops,
    int Index,
    int Limit
) : ScopeState()
{
    public static DoScopeState Create(int index, int limit) => new(
        ImmutableList<string>.Empty,
        NestedLoops: 0,
        Index: index,
        Limit: limit
    );

    public DoScopeState IncIndex() => ReachedLimit()
        ? throw new Exception("Index out of limit.")
        : this with { Index = Index + 1 };

    public bool ReachedLimit() => Index >= Limit - 1;

    public DoScopeState AddWord(string word) => this with { LoopWords = LoopWords.Add(word) };

    public DoScopeState IncNestedLoops() => this with { NestedLoops = NestedLoops + 1 };

    public DoScopeState DecNestedLoops() => NestedLoops > 0
        ? this with { NestedLoops = NestedLoops + 1 }
        : throw new Exception("NestedLoops value cannot be less zero.");
}