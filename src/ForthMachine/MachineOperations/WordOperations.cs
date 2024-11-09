using DFAutomaton;

namespace ForthMachine;

public static class WordOperations
{
    public static ReductionResult<string, MachineState> StartWordDefinition(MachineState state, string _) =>
        state.PushScope(WordDefinitionScope.Initial);

    public static ReductionResult<string, MachineState> BeginWord(MachineState state, string word) =>
        state.PopScope(out WordDefinitionScope scope).PushScope(scope.SetWord(word));

    public static Reduce<string, MachineState> AddInnerWord(StateId gotoStateId)
    {
        return (state, innerWord) =>
        {
            var nextState = state.PopScope(out WordDefinitionScope scope)
                .PushScope(scope.AddInnerWord(innerWord));

            var result = new ReductionResult<string, MachineState>(nextState);
            return result.DynamiclyGoTo(gotoStateId);
        };
    }

    public static ReductionResult<string, MachineState> FinishWordDefinition(MachineState state, string _)
    {
        var state2 = state.PopScope(out SyntacticScope scope);
        if (scope is not WordDefinitionScope wordScope)
            return state2.SetError("Unexpected ;");

        var (word, innerWord) = wordScope;
        return state2.NewWord(word, innerWord);
    }

    public static Reduce<string, MachineState> ExecWord(StateId gotoStateId)
    {
        return (state, word) =>
        {
            if (!state.DefinedWords.TryGetValue(word, out var innerWords))
                return state.SetError($"Unexpected word {word}.");

            var result = new ReductionResult<string, MachineState>(state);
            foreach (var innerWord in innerWords)
                result.YieldNext(innerWord);
            
            return result.DynamiclyGoTo(gotoStateId);
        };
    }
}