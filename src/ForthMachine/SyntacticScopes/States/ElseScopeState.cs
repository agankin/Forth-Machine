namespace ForthMachine;

internal record ElseScopeState(
    SyntacticScopeTracker ScopeTracker
) : ScopeState()
{
    public static readonly ElseScopeState Initial = new(new SyntacticScopeTracker());
}