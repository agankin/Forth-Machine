using System.Collections;
using System.Collections.Immutable;

namespace ForthMachine;

public record SyntacticScopeStack : IEnumerable<ScopeState>
{
    public SyntacticScopeStack(ScopeState rootScope) => RootScope = rootScope;

    private ImmutableStack<ScopeState> InternalStack { get; init; } = ImmutableStack<ScopeState>.Empty;

    private ScopeState RootScope { get; init; }

    public SyntacticScopeStack Push(ScopeState scope) => this with { InternalStack = InternalStack.Push(scope) };

    public SyntacticScopeStack Pop(out ScopeState scope)
    {
        scope = RootScope;

        return InternalStack.IsEmpty
            ? this
            : this with { InternalStack = InternalStack.Pop(out scope) };
    }

    public IEnumerator<ScopeState> GetEnumerator() => ((IEnumerable<ScopeState>)InternalStack).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}