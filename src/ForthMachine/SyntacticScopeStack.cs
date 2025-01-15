using System.Collections;
using System.Collections.Immutable;

namespace ForthMachine;

public record SyntacticScopeStack : IEnumerable<SyntacticScope>
{
    public SyntacticScopeStack(SyntacticScope rootScope) => RootScope = rootScope;

    private ImmutableStack<SyntacticScope> InternalStack { get; init; } = ImmutableStack<SyntacticScope>.Empty;

    private SyntacticScope RootScope { get; init; }

    public SyntacticScopeStack Push(SyntacticScope scope) => this with { InternalStack = InternalStack.Push(scope) };

    public SyntacticScopeStack Pop(out SyntacticScope scope)
    {
        scope = RootScope;

        return InternalStack.IsEmpty
            ? this
            : this with { InternalStack = InternalStack.Pop(out scope) };
    }

    public IEnumerator<SyntacticScope> GetEnumerator() => ((IEnumerable<SyntacticScope>)InternalStack).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}