using DFAutomaton;
using PureMonads;

namespace ForthMachine;

public static class PrintOperations
{
    public static ReductionResult<string, MachineState> PopPrint(MachineState state, string _)
    {
        var nextState = state.Pop(out var value);        
        nextState.Error.OnNone(() => Print(value?.ToString() ?? string.Empty));
        
        return nextState;
    }
    
    public static ReductionResult<string, MachineState> Stack(MachineState state, string _)
    {
        foreach (var value in state.Stack)
            Print(value.ToString());
        
        return state;
    }

    public static ReductionResult<string, MachineState> ScopeStack(MachineState state, string _)
    {
        foreach (var scope in state.SyntacticScopeStack)
            Print(scope.ToString());
        
        return state;
    }

    private static void Print(string value)
    {
        var color = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(value);
        }
        finally { Console.ForegroundColor = color; }
    }
}