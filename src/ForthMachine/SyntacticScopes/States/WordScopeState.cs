using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

internal record WordScopeState(
    string Name,
    ImmutableList<string> BodyWords,
    SyntacticScopeTracker BodyScopeTracker
) : ScopeState()
{
    public static readonly WordScopeState Initial = new(
        Name: null!,
        BodyWords: ImmutableList<string>.Empty,
        BodyScopeTracker: new()
    );

    public bool HasInnerScope => BodyScopeTracker.HasInnerScope;

    public WordScopeState SetName(string name) => this with { Name = name };

    public Result<WordScopeState, string> AddBodyWord(string word)
    {
        return BodyScopeTracker.TrackNext(word)
            .Map(scopeTracker =>
                this with
                {
                    BodyWords = BodyWords.Add(word),
                    BodyScopeTracker = scopeTracker
                });
    }
}