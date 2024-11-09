using DFAutomaton;

namespace ForthMachine;

public static class CmpOperations
{
    public static ReductionResult<string, MachineState> Eq(MachineState state, string _) =>
        state.Pop(out var value2).Pop(out var value1)
            .Map(state => value1?.GetType() == value2?.GetType()
                ? state
                : state.SetError($"Compared values {value2} and {value1} have different types."))
            .Map(state => state.Push(value1 == value2));

    public static ReductionResult<string, MachineState> NotEq(MachineState state, string _) =>
        state.Pop(out var value2).Pop(out var value1)
            .Map(state => value1?.GetType() == value2?.GetType()
                ? state
                : state.SetError($"Compared values {value2} and {value1} have different types."))
            .Map(state => state.Push(value1 != value2));

    public static ReductionResult<string, MachineState> Less(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 < value2));
    
    public static ReductionResult<string, MachineState> LessOrEq(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 <= value2));

    public static ReductionResult<string, MachineState> Greater(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 > value2));
    
    public static ReductionResult<string, MachineState> GreaterOrEq(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 >= value2));
}