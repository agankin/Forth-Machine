using DFAutomaton;

namespace ForthMachine;

public static class IfOperations
{
    public static Reduce<string, MachineState> If(StateId trueStateId, StateId falseStateId)
    {
        return (MachineState machineState, string _) =>
        {
            var nextMachineState = machineState.Pop(out bool condition).PushScope(new IfScope());
                return new ReductionResult<string, MachineState>(nextMachineState)
                    .DynamiclyGoTo(condition ? trueStateId : falseStateId);
        };
    }
    
    public static Reduce<string, MachineState> Else(StateId elseStateId)
    {
        return (MachineState state, string _) =>
        {
            var nextState = state.PopScope(out SyntacticScope scope);

            return scope is IfScope
                ? new ReductionResult<string, MachineState>(nextState.PushScope(new ElseScope()))
                    .DynamiclyGoTo(elseStateId)
                : state.SetError("Unexpected ELSE.");
        };
    }

    public static ReductionResult<string, MachineState> Then(MachineState state, string _)
    {
        var nextState = state.PopScope(out SyntacticScope scope);

        return scope is IfScope || scope is ElseScope
            ? nextState
            : state.SetError("Unexpected THEN.");
    }
}