using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

internal record BeginScopeState(
    ImmutableList<string> BodyWords,
    SyntacticScopeTracker InnerScopeTracker
) : ScopeState()
{
    public static readonly BeginScopeState Initial = new(
        ImmutableList<string>.Empty,
        InnerScopeTracker: new()
    );

    public bool HasInnerScope => !InnerScopeTracker.Scopes.IsEmpty;

    public Result<BeginScopeState, string> AddBodyWord(string word)
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