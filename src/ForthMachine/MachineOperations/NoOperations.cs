using DFAutomaton;

namespace ForthMachine;

public static class NoOperations
{
    public static ReductionResult<string, MachineState> NoOp(MachineState state, string _) => state;
}