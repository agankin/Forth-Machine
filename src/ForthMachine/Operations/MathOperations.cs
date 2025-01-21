using DFAutomaton;

namespace ForthMachine;

public static class MathOperations
{
    public static ReductionResult<string, MachineState> Add(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => state.Push(number1 + number2));

    public static ReductionResult<string, MachineState> Sub(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => state.Push(number1 - number2));

    public static ReductionResult<string, MachineState> Mul(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => state.Push(number1 * number2));

    public static ReductionResult<string, MachineState> Div(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => number2 != 0
                ? state.Push(number1 / number2)
                : state.SetError("Division by zero."));

    public static ReductionResult<string, MachineState> Neg(MachineState state, string _) =>
        state.Pop(out decimal number)
            .Map(state => state.Push(-1 * number));

    public static ReductionResult<string, MachineState> Abs(MachineState state, string _) =>
        state.Pop(out decimal number)
            .Map(state => state.Push(Math.Abs(number)));
}