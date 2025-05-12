using DFAutomaton;

namespace ForthMachine;

public static class DefineWordOperations
{
    public static ReductionResult<string, MachineState> BeginWord(MachineState state, string _)
    {
        return state.HasInnerScope
            ? state.Unexpected(MachineWords.BeginWord)
            : state.PushScope(WordScopeState.Initial);
    }

    public static ReductionResult<string, MachineState> SetName(MachineState state, string name) =>
        state.MapTopScope<WordScopeState>(scope => scope.SetName(name));

    public static ReductionResult<string, MachineState> ProcessBody(MachineState state, string word)
    {
        if (word == MachineWords.BeginWord)
            return state.Unexpected(MachineWords.BeginWord);

        return state.FlatMapTopScope<WordScopeState>(scope => scope.AddBodyWord(word));
    }

    public static ReductionResult<string, MachineState> EndWord(MachineState state, string _)
    {
        var nextState = state.PopScope(out WordScopeState scope);
        if (scope.HasInnerScope)
            return nextState.Unexpected(MachineWords.EndWord);

        var (name, bodyWords, _) = scope;

        return nextState.AddWord(name, bodyWords);
    }

    public static ReductionResult<string, MachineState> CallWord(MachineState state, string word)
    {
        if (!state.DefinedWords.TryGetValue(word, out var bodyWords))
            return state.Unexpected(word);

        var result = new ReductionResult<string, MachineState>(state);
        bodyWords.ForEach(result.YieldNext);
        
        return result;
    }
}