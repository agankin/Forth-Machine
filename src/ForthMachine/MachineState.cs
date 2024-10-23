using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

using static Option;

public record MachineState(
    ImmutableStack<decimal> Stack,
    Option<string> Error
)
{
    public static MachineState Initial => new MachineState(
        Stack: ImmutableStack<decimal>.Empty,
        Error: None<string>()
    );

    public MachineState Push(decimal number) => MapValid(
        state => state with { Stack = Stack.Push(number) }
    );

    public MachineState Pop(out decimal number)
    {
        decimal localNumber = default;
        var newState = MapValid(state => Stack.IsEmpty
            ? state.SetError("Stack is empty!")
            : state with { Stack = Stack.Pop(out localNumber) }
        );

        number = localNumber;
        return newState;
    }

    public MachineState SetError(string error) => MapValid(
        state => state with { Error = error }
    );

    private MachineState MapValid(Func<MachineState, MachineState> map) => Error.Match(_ => this, () => map(this));
}