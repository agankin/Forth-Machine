using DFAutomaton;

namespace ForthMachine;

public static class DoLoopOperations
{
    public static ReductionResult<string, MachineState> Do(MachineState state, string _)
    {
        return state
            .Pop(out decimal index)
            .Pop(out decimal limit)
            .PushScope(DoScopeState.Create((int)index, (int)limit));
    }

    public static ReductionResult<string, MachineState> ProcessBody(MachineState state, string word) =>
        state
            .PopScope(out DoScopeState doLoopScope)
            .PushScope(doLoopScope.AddWord(word));

    public static ReductionResult<string, MachineState> Loop(MachineState state, string _)
    {
        ReductionResult<string, MachineState> result = state
            .PopScope(out DoScopeState doLoopScope)
            .PushScope(doLoopScope);
        
        doLoopScope.LoopWords.Append(MachineWords.Loop).ForEach(result.YieldNext);
        
        return result;
    }

    public static ReductionResult<string, MachineState> Repeat(MachineState state, string _)
    {
        var state2 = state.PopScope(out ScopeState scope);
        if (scope is not DoScopeState doLoopScope)
            return state2.Unexpected(MachineWords.Loop);

        if (doLoopScope.ReachedLimit())
            return state2;

        var doLoopScope2 = doLoopScope.IncIndex();
        var result = new ReductionResult<string, MachineState>(state2.PushScope(doLoopScope2));
        
        doLoopScope.LoopWords.Append(MachineWords.Loop).ForEach(result.YieldNext);

        return result;
    }
}