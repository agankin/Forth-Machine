using DFAutomaton;

namespace ForthMachine;

public static class BeginLoopOperations
{
    public static ReductionResult<string, MachineState> Begin(MachineState state, string _) =>
        state.PushScope(BeginLoopScope.Empty);

    public static Reduce<string, MachineState> BeginLoop(StateId gotoStateId)
    {
        return (MachineState state, string word) =>
        {
            var state2 = state.PopScope(out SyntacticScope scope);
            var beginScope = (BeginLoopScope)scope;
            
            var state3 = state2.PushScope(beginScope.AddWord(word));

            return new ReductionResult<string, MachineState>(state3)
                .DynamiclyGoTo(gotoStateId);
        };
    }

    public static ReductionResult<string, MachineState> BeginFinish(MachineState state, string _)
    {
        var state2 = state.PopScope(out SyntacticScope scope);
        var beginScope = (BeginLoopScope)scope;
        
        var result = new ReductionResult<string, MachineState>(state2.PushScope(beginScope));
        foreach (var word in beginScope.LoopWords.Append("UNTIL"))
            result.YieldNext(word);
        
        return result;
    }

    public static ReductionResult<string, MachineState> Until(MachineState state, string _)
    {
        var state2 = state.PopScope(out SyntacticScope scope);
        if (scope is not BeginLoopScope beginScope)
            return state2.SetError("Unexpected UNTIL.");

        var state3 = state2.Pop(out bool condition);
        if (state3.Error.HasValue)
            return state3;

        if (condition)
            return state3;

        var result = new ReductionResult<string, MachineState>(state3.PushScope(beginScope));
        foreach (var word in beginScope.LoopWords.Append("UNTIL"))
            result.YieldNext(word);

        return result;
    }
}