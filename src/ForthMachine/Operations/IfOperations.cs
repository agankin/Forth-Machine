using DFAutomaton;

namespace ForthMachine;

public static class IfOperations
{
    public static Reduce<string, MachineState> BeginIf(StateId trueStateId, StateId falseStateId)
    {
        return (MachineState machineState, string _) =>
        {
            var nextMachineState = machineState.Pop(out bool condition).PushScope(new IfScopeState());
                return new ReductionResult<string, MachineState>(nextMachineState)
                    .DynamiclyGoTo(condition ? trueStateId : falseStateId);
        };
    }
    
    public static Reduce<string, MachineState> BeginElse(StateId elseStateId)
    {
        return (MachineState state, string _) =>
        {
            var nextState = state.PopScope(out ScopeState scope);

            return scope is IfScopeState
                ? new ReductionResult<string, MachineState>(nextState.PushScope(new ElseScopeState()))
                    .DynamiclyGoTo(elseStateId)
                : state.SetError("Unexpected 'ELSE' word.");
        };
    }

    public static ReductionResult<string, MachineState> EndIf(MachineState state, string _)
    {
        var nextState = state.PopScope(out ScopeState scope);

        return scope is IfScopeState || scope is ElseScopeState
            ? nextState
            : state.SetError("Unexpected 'THEN' word.");
    }
}