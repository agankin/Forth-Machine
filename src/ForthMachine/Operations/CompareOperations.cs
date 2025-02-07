using DFAutomaton;

namespace ForthMachine;

public static class CompareOperations
{
    public static ReductionResult<string, MachineState> Eq(MachineState state, string _) =>
        state.Pop(out var value2).Pop(out var value1)
            .FlattenError(() => CompatibilityChecker.Check(value1!, value2!))
            .Map(state => state.Push(value1 == value2));

    public static ReductionResult<string, MachineState> NotEq(MachineState state, string _) =>
        state.Pop(out var value2).Pop(out var value1)
            .FlattenError(() => CompatibilityChecker.Check(value1!, value2!))
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