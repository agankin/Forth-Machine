using System.Globalization;
using DFAutomaton;

namespace ForthMachine;

public static class StackOperations
{
    public static ReductionResult<string, MachineState> PushNumber(MachineState state, string numberWord)
    {
        var number = decimal.Parse(numberWord, CultureInfo.InvariantCulture);
        return state.Push(number);
    }

    public static ReductionResult<string, MachineState> PushFalse(MachineState state, string _) => state.Push(false);

    public static ReductionResult<string, MachineState> PushTrue(MachineState state, string _) => state.Push(true);

    public static ReductionResult<string, MachineState> Depth(MachineState state, string _) =>
        state.Push(state.Stack.Count());
    
    public static ReductionResult<string, MachineState> Dup(MachineState state, string _) =>
        state.Pop(out var value).Push(value!).Push(value!);

    public static ReductionResult<string, MachineState> Swap(MachineState state, string _) =>
        state.Pop(out var value1).Pop(out var value2).Push(value1!).Push(value2!);

    public static ReductionResult<string, MachineState> Over(MachineState state, string _) =>
        state.Pop(out var value1).Pop(out var value2).Push(value2!).Push(value1!).Push(value2!);

    public static ReductionResult<string, MachineState> Drop(MachineState state, string _) => state.Pop(out var _);
}