using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

using static Option;

public record MachineState(
    ImmutableStack<MachineValue> Stack,
    ImmutableStack<SyntacticScope> SyntacticScopeStack,
    ImmutableDictionary<string, ImmutableList<string>> DefinedWords,
    Option<string> Error
)
{
    public static MachineState Initial => new MachineState(
        Stack: ImmutableStack<MachineValue>.Empty,
        SyntacticScopeStack: ImmutableStack<SyntacticScope>.Empty,
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
            ? state.SetError("Stack is empty!")
            : state with { Stack = Stack.Pop(out localValue) }
        );

        value = localValue;
        return newState;
    }

    public MachineState PushScope(SyntacticScope scope) => Map(
        state => state with { SyntacticScopeStack = SyntacticScopeStack.Push(scope) }
    );

    public MachineState PopScope(out SyntacticScope scope)
    {
        SyntacticScope localScope = new NoneScope();
        var newState = Map(state => SyntacticScopeStack.IsEmpty
            ? state
            : state with { SyntacticScopeStack = SyntacticScopeStack.Pop(out localScope) }
        );

        scope = localScope;
        return newState;
    }

    public MachineState NewWord(string word, ImmutableList<string> innerWord) => Map(
        state => state with { DefinedWords = DefinedWords.Add(word, innerWord) }
    );

    public MachineState SetError(string error) => Map(
        state => state with { Error = error }
    );

    public MachineState Map(Func<MachineState, MachineState> map) => Error.Match(_ => this, () => map(this));
}