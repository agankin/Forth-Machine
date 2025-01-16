using System.Text;
using DFAutomaton;
using PureMonads;

namespace ForthMachine.Repl;

public static class ReplWriter
{
    public static void PrintOk(long execTimeMS) => Print($"Ok. {execTimeMS} ms.", ConsoleColor.Green);
    
    public static void PrintError(AutomatonError<string, MachineState> error) => Print(error.Format(), ConsoleColor.Red);

    private static void Print(string line, ConsoleColor color)
    {
        var prevColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
        }
        finally
        {
            Console.ForegroundColor = prevColor;
        }
    }

    private static string Format(this AutomatonError<string, MachineState> error)
    {
        var stringBuilder = new StringBuilder();

        if (error.Type == AutomatonErrorType.StateError)
        {
            var errorDescription = error.StateValue.Error.ValueOrFailure();
            stringBuilder.Append($"ERROR: {errorDescription}");
        }
        else
        {
            stringBuilder.AppendLine("ERROR:");
            stringBuilder.AppendLine($"Type = {error.Type}");
            var fromStateId = error.WhenTransitioningFrom.Map(state => $"StateId: {state.Id}");
            stringBuilder.AppendLine($"WhenTransitioningFrom = {fromStateId}");
            stringBuilder.Append($"Transition = {error.Transition}");
        }

        return stringBuilder.ToString();
    }
}