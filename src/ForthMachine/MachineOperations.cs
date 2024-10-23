using System.Globalization;
using DFAutomaton;
using PureMonads;

namespace ForthMachine;

public static class MachineOperations
{
    public static ReductionResult<string, MachineState> PushNumber(MachineState state, string numberWord) =>
        state.Push(decimal.Parse(numberWord, CultureInfo.InvariantCulture));

    public static ReductionResult<string, MachineState> Add(MachineState state, string _) =>
        state.Pop(out var num2).Pop(out var num1).Push(num1 + num2);

    public static ReductionResult<string, MachineState> Sub(MachineState state, string _) =>
        state.Pop(out var num2).Pop(out var num1).Push(num1 - num2);

    public static ReductionResult<string, MachineState> Mul(MachineState state, string _) =>
        state.Pop(out var num2).Pop(out var num1).Push(num1 * num2);

    public static ReductionResult<string, MachineState> Div(MachineState state, string _) =>
        state.Pop(out var num2).Pop(out var num1)
            .Pipe(state => num2 != 0
                ? state.Push(num1 / num2)
                : state.SetError("Division by zero!"));

    public static ReductionResult<string, MachineState> PopNumber(MachineState state, string _)
    {
        var nextState = state.Pop(out var number);
        nextState.Error.OnNone(() => PrintResult(number));
        
        return nextState;
    }

    private static void PrintResult(decimal result)
    {
        var color = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(result);
        }
        finally { Console.ForegroundColor = color; }
    }
}