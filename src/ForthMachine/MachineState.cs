using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

using static Option;

public record MachineState(
    ImmutableStack<MachineValue> Stack,
    ImmutableStack<ScopeState> SyntacticScopeStack,
    ImmutableDictionary<string, ImmutableList<string>> DefinedWords,
    Option<string> Error
)
{
    public static MachineState Initial => new MachineState(
        Stack: ImmutableStack<MachineValue>.Empty,
        SyntacticScopeStack: ImmutableStack<ScopeState>.Empty,
        DefinedWords: ImmutableDictionary<string, ImmutableList<string>>.Empty,
        Error: None<string>()
    );

    public MachineState Push(MachineValue value) => Map(
        state => state with { Stack = Stack.Push(value) }
    );

    public MachineState Pop(out MachineValue? value)
    {
        MachineValue? localValue = null;
        var newState = Map(state => Stack.IsEmpty
            ? state.SetError("Stack is empty.")
            : state with { Stack = Stack.Pop(out localValue) }
        );

        value = localValue;
        return newState;
    }

    public MachineState MapScope(Func<ImmutableStack<ScopeState>, ImmutableStack<ScopeState>> map) => Map(
        state => state with { SyntacticScopeStack = map(SyntacticScopeStack) }
    );

    public MachineState MapScope(Func<ImmutableStack<ScopeState>, Result<ImmutableStack<ScopeState>, string>> map) => Map(
        state => map(state.SyntacticScopeStack)
            .Match(
                nextScopeStack => state with { SyntacticScopeStack = nextScopeStack },
                state.SetError
            )
    );

    public MachineState PushScope(ScopeState scope) => Map(
        state => state with { SyntacticScopeStack = SyntacticScopeStack.Push(scope) }
    );

    public MachineState PopScope(out ScopeState scope)
    {
        ScopeState localScope = new RootScopeState();
        var nextState = Map(state =>
            state with
            {
                SyntacticScopeStack = SyntacticScopeStack.PopOrDefault(RootScopeState.Instance, out localScope)
            });

        scope = localScope;
        return nextState;
    }

    public MachineState AddWord(string word, ImmutableList<string> innerWord) => Map(
        state => state with { DefinedWords = DefinedWords.Add(word, innerWord) }
    );

    public MachineState SetError(string error) => Map(
        state => state with { Error = error }
    );

    public MachineState Map(Func<MachineState, MachineState> map) => Error.Match(_ => this, () => map(this));
}