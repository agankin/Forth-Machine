using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

internal record BeginScopeState(
    ImmutableList<string> BodyWords,
    SyntacticScopeTracker BodyScopeTracker
) : ScopeState()
{
    public static readonly BeginScopeState Initial = new(
        ImmutableList<string>.Empty,
        BodyScopeTracker: new()
    );

    public bool HasInnerScope => BodyScopeTracker.HasInnerScope;

    public Result<BeginScopeState, string> AddBodyWord(string word)
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