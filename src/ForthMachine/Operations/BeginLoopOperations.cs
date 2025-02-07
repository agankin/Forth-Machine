using DFAutomaton;

namespace ForthMachine;

public static class BeginLoopOperations
{
    public static ReductionResult<string, MachineState> Begin(MachineState state, string _) =>
        state.PushScope(BeginScopeState.Initial);

    public static ReductionResult<string, MachineState> ProcessBody(MachineState state, string word) =>
        state.FlatMapTopScope<BeginScopeState>(scope => scope.AddBodyWord(word));

    public static ReductionResult<string, MachineState> Until(MachineState state, string _)
    {
        var scope = (BeginScopeState)state.PeekScope();
        if (scope.HasInnerScope)
            return state.Unexpected(MachineWords.Until);
        
        return Repeat(state, scope);
    }

    public static ReductionResult<string, MachineState> Repeat(MachineState state, string _)
    {
        var nextState = state.Pop(out bool condition);
        if (condition || !nextState.Valid())
            return nextState;

        var scope = (BeginScopeState)state.PeekScope();

        return Repeat(nextState, scope);
    }

    private static ReductionResult<string, MachineState> Repeat(MachineState state, BeginScopeState scope)
    {
        var result = new ReductionResult<string, MachineState>(state);
        scope.BodyWords.Append(MachineWords.Until).ForEach(result.YieldNext);

        return result;
    }
}