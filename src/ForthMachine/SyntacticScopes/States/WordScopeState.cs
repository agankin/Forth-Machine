using System.Collections.Immutable;

namespace ForthMachine;

internal record WordScopeState(
    string Word,
    ImmutableList<string> InnerWords
) : ScopeState()
{
    public static readonly WordScopeState Initial = new(null!, ImmutableList<string>.Empty);

    public WordScopeState SetWord(string word) => this with { Word = word };

    public WordScopeState AddInnerWord(string innerWord) => this with { InnerWords = InnerWords.Add(innerWord) };
}