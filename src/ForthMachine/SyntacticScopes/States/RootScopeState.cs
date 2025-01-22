namespace ForthMachine;

internal record RootScopeState() : ScopeState()
{
    public static readonly RootScopeState Instance = new();
}