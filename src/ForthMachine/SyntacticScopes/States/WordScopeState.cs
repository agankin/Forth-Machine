using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

internal record WordScopeState(
    string Name,
    ImmutableList<string> BodyWords,
    SyntacticScopeTracker InnerScopeTracker
) : ScopeState()
{
    public static readonly WordScopeState Initial = new(
        Name: null!,
        BodyWords: ImmutableList<string>.Empty,
        InnerScopeTracker: new()
    );

    public bool HasInnerScope => !InnerScopeTracker.Scopes.IsEmpty;

    public WordScopeState SetName(string name) => this with { Name = name };

    public Result<WordScopeState, string> AddBodyWord(string word)
    {
        return InnerScopeTracker.OnNextWord(word)
            .Map(scopeTracker =>
                this with
                {
                    BodyWords = BodyWords.Add(word),
                    InnerScopeTracker = scopeTracker
                });
    }
}