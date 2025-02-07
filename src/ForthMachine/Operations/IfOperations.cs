using DFAutomaton;

namespace ForthMachine;

public static class IfOperations
{
    public static Reduce<string, MachineState> If(StateId trueStateId, StateId falseStateId)
    {
        return (MachineState machineState, string _) =>
        {
            var nextMachineState = machineState.Pop(out bool condition).PushScope(IfScopeState.Initial);
            var goToStateId = condition ? trueStateId : falseStateId;
            
            return new ReductionResult<string, MachineState>(nextMachineState).DynamiclyGoTo(goToStateId);
        };
    }
    
    public static Reduce<string, MachineState> Else(StateId elseStateId)
    {
        return (MachineState state, string _) =>
        {
            var nextState = state.PopScope(out ScopeState scope);
            if (scope is not IfScopeState ifScope)
                return state.Unexpected(MachineWords.Else);

            return new ReductionResult<string, MachineState>(nextState.PushScope(ElseScopeState.Initial))
                .DynamiclyGoTo(elseStateId);
        };
    }

    public static ReductionResult<string, MachineState> Then(MachineState state, string _)
    {
        var nextState = state.PopScope(out ScopeState scope);
        if (scope is not IfScopeState or ElseScopeState)
            return state.Unexpected(MachineWords.Then);

        return nextState;
    }
}