using DFAutomaton;

namespace ForthMachine;

public static class BeginLoopOperations
{
    public static ReductionResult<string, MachineState> BeginLoop(MachineState state, string _) =>
        state.PushScope(BeginScopeState.Empty);

    public static ReductionResult<string, MachineState> AddInnerWord(MachineState state, string word) =>
        state
            .PopScope(out BeginScopeState beginLoopScope)
            .PushScope(beginLoopScope.AddWord(word));

    public static ReductionResult<string, MachineState> EndLoop(MachineState state, string _)
    {
        var state2 = state.PopScope(out BeginScopeState beginLoopScope);
        
        var result = new ReductionResult<string, MachineState>(state2.PushScope(beginLoopScope));
        foreach (var word in beginLoopScope.LoopWords.Append("UNTIL"))
            result.YieldNext(word);
        
        return result;
    }

    public static ReductionResult<string, MachineState> Repeat(MachineState state, string _)
    {
        var state2 = state.PopScope(out ScopeState scope);
        if (scope is not BeginScopeState beginLoopScope)
            return state2.SetError("Unexpected 'UNTIL' word.");

        var state3 = state2.Pop(out bool condition);
        if (!state3.Valid())
            return state3;

        if (condition)
            return state3;

        var result = new ReductionResult<string, MachineState>(state3.PushScope(beginLoopScope));
        foreach (var word in beginLoopScope.LoopWords.Append("UNTIL"))
            result.YieldNext(word);

        return result;
    }
}