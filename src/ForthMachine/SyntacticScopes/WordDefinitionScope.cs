using System.Collections.Immutable;

namespace ForthMachine;

public record WordDefinitionScope(
    string Word,
    ImmutableList<string> InnerWords
) : SyntacticScope()
{
    public static readonly WordDefinitionScope Initial = new(null!, ImmutableList<string>.Empty);

    public WordDefinitionScope SetWord(string word) => this with { Word = word };

    public WordDefinitionScope AddInnerWord(string innerWord) => this with { InnerWords = InnerWords.Add(innerWord) };
}