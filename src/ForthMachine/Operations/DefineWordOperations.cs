using DFAutomaton;

namespace ForthMachine;

public static class DefineWordOperations
{
    public static ReductionResult<string, MachineState> BeginWordDefinition(MachineState state, string _)
    {
        state.PopScope(out var scope);
        if (scope is not RootScopeState)
            return state.SetError($"Unexpected ':' word.");

        return state.PushScope(WordScopeState.Initial);
    }

    public static ReductionResult<string, MachineState> SetWord(MachineState state, string word) =>
        state.PopScope(out WordScopeState scope).PushScope(scope.SetWord(word));

    public static ReductionResult<string, MachineState> AddInnerWord(MachineState state, string word)
    {
        if (word == MachineWords.BeginWord)
            return state.SetError($"Unexpected ':' word.");

        return state.PopScope(out WordScopeState scope)
            .PushScope(scope.AddInnerWord(word));
    }

    public static ReductionResult<string, MachineState> EndWordDefinition(MachineState state, string _)
    {
        var state2 = state.PopScope(out ScopeState scope);
        if (scope is not WordScopeState wordScope)
            return state2.SetError("Unexpected ';' word.");

        var (word, innerWord) = wordScope;
        return state2.AddWord(word, innerWord);
    }

    public static ReductionResult<string, MachineState> ExecWord(MachineState state, string word)
    {
        if (!state.DefinedWords.TryGetValue(word, out var innerWords))
            return state.SetError($"Unexpected '{word}' word.");

        var result = new ReductionResult<string, MachineState>(state);
        foreach (var innerWord in innerWords)
            result.YieldNext(innerWord);
        
        return result;
    }
}