using System.Text;
using DFAutomaton;
using PureMonads;

namespace ForthMachine;

public static class MachineErrorFormatter
{
    public static string Format(this AutomatonError<string, MachineState> error)
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