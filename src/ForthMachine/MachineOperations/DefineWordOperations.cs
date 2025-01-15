using DFAutomaton;

namespace ForthMachine;

public static class DefineWordOperations
{
    public static ReductionResult<string, MachineState> BeginWordDefinition(MachineState state, string _)
    {
        state.PopScope(out var scope);
        if (scope is not NoneScope)
            return state.SetError($"Unexpected ':' word.");

        return state.PushScope(WordDefinitionScope.Initial);
    }

    public static ReductionResult<string, MachineState> SetWord(MachineState state, string word) =>
        state.PopScope(out WordDefinitionScope scope).PushScope(scope.SetWord(word));

    public static ReductionResult<string, MachineState> AddInnerWord(MachineState state, string word)
    {
        if (word == ":")
            return state.SetError($"Unexpected ':' word.");

        return state.PopScope(out WordDefinitionScope scope)
            .PushScope(scope.AddInnerWord(word));
    }

    public static ReductionResult<string, MachineState> EndWordDefinition(MachineState state, string _)
    {
        var state2 = state.PopScope(out SyntacticScope scope);
        if (scope is not WordDefinitionScope wordScope)
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