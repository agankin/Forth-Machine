using DFAutomaton;

namespace ForthMachine;

public static class DoLoopOperations
{
    public static ReductionResult<string, MachineState> BeginLoop(MachineState state, string _)
    {
        return state
            .Pop(out decimal index)
            .Pop(out decimal limit)
            .PushScope(DoLoopScope.Create((int)index, (int)limit));
    }

    public static ReductionResult<string, MachineState> AddInnerWord(MachineState state, string word) =>
        state
            .PopScope(out DoLoopScope doLoopScope)
            .PushScope(doLoopScope.AddWord(word));

    public static ReductionResult<string, MachineState> EndLoop(MachineState state, string _)
    {
        ReductionResult<string, MachineState> result = state
            .PopScope(out DoLoopScope doLoopScope)
            .PushScope(doLoopScope);
        
        foreach (var word in doLoopScope.LoopWords.Append("LOOP"))
            result.YieldNext(word);
        
        return result;
    }

    public static ReductionResult<string, MachineState> Repeat(MachineState state, string _)
    {
        var state2 = state.PopScope(out SyntacticScope scope);
        if (scope is not DoLoopScope doLoopScope)
            return state2.SetError("Unexpected 'LOOP' word.");

        if (doLoopScope.ReachedLimit())
            return state2;

        var doLoopScope2 = doLoopScope.IncIndex();
        var result = new ReductionResult<string, MachineState>(state2.PushScope(doLoopScope2));
        
        foreach (var word in doLoopScope.LoopWords.Append("LOOP"))
            result.YieldNext(word);

        return result;
    }
}