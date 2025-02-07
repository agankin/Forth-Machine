namespace ForthMachine;

internal record IfScopeState(
    SyntacticScopeTracker ScopeTracker
) : ScopeState()
{
    public static readonly IfScopeState Initial = new(new SyntacticScopeTracker());
}