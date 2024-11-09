using DFAutomaton;

namespace ForthMachine;

public static class NoOperations
{
    public static ReductionResult<string, MachineState> NoOp(MachineState state, string _) => state;

    public static Reduce<string, MachineState> NoOp(StateId goToStateId) =>
        (state, _) => new ReductionResult<string, MachineState>(state).DynamiclyGoTo(goToStateId);
}