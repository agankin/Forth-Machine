using System.Collections.Immutable;

namespace ForthMachine;

internal record BeginScopeState(
    ImmutableList<string> LoopWords,
    int NestedLoops
) : ScopeState()
{
    public static readonly BeginScopeState Empty = new(
        ImmutableList<string>.Empty,
        NestedLoops: 0
    );

    public BeginScopeState AddWord(string word) => this with { LoopWords = LoopWords.Add(word) };

    public BeginScopeState IncNestedLoops() => this with { NestedLoops = NestedLoops + 1 };

    public BeginScopeState DecNestedLoops() => NestedLoops > 0
        ? this with { NestedLoops = NestedLoops + 1 }
        : throw new Exception("NestedLoops value cannot be less zero.");
}