namespace ForthMachine;

using System.Globalization;
using DFAutomaton;
using static MachineOperations;

public static class MachineAutomatonBuilder
{
    public static Automaton<string, MachineState> Build()
    {
        var builder = new AutomatonBuilder<string, MachineState>(StringComparer.OrdinalIgnoreCase);
        var start = builder.Start;

        start.TransitsBy("RESET")
            .WithReducingTo(MachineState.Initial)
            .ToSelf();
        
        bool IsNumber(string word) => decimal.TryParse(word, CultureInfo.InvariantCulture, out _);
        
        start.TransitsWhen(IsNumber)
            .WithReducingBy(PushNumber)
            .ToSelf();

        start.TransitsBy(".")
            .WithReducingBy(PopNumber)
            .To(start);

        start.TransitsBy("+").WithReducingBy(Add).ToSelf();
        start.TransitsBy("-").WithReducingBy(Sub).ToSelf();
        start.TransitsBy("*").WithReducingBy(Mul).ToSelf();
        start.TransitsBy("/").WithReducingBy(Div).ToSelf();

        start.TransitsBy("END")
            .WithReducingBy((state, _) => state)
            .ToAccepted();

        return builder
            .ValidateAnyCanReachAccepted()
            .AddCheckForErrorState(state => state.Error.HasValue)
            .Build()
            .Match(automaton => automaton, error => throw new Exception($"Error: {error}."));
    }
}