namespace ForthMachine;

using System.Globalization;
using DFAutomaton;

using static MachineStateReducers;

public static class MachineBuilder
{
    public static Automaton<string, MachineState> Build()
    {
        var builder = new AutomatonBuilder<string, MachineState>(StringComparer.OrdinalIgnoreCase);
        var start = builder.Start;
        
        AddOperations(start);
        AddIfElseThen(start, builder);
        
        start.TransitsBy("END").WithReducingBy(NoOp).ToAccepted();
        
        return builder
            .ValidateAnyCanReachAccepted()
            .AddCheckForErrorState(state => state.Error.HasValue)
            .Build()
            .Match(automaton => automaton, error => throw new Exception($"Error: {error}."));
    }

    private static bool IsNumber(string word) => decimal.TryParse(word, CultureInfo.InvariantCulture, out var _);

    private static void AddOperations(State<string, MachineState> state)
    {
        state.TransitsWhen(IsNumber).WithReducingBy(PushNumber).ToSelf();
        state.TransitsBy("FALSE").WithReducingBy(PushFalse).ToSelf();
        state.TransitsBy("TRUE").WithReducingBy(PushTrue).ToSelf();
        
        state.TransitsBy(".").WithReducingBy(PopPrint).ToSelf();
        
        state.TransitsBy("+").WithReducingBy(Add).ToSelf();
        state.TransitsBy("-").WithReducingBy(Sub).ToSelf();
        state.TransitsBy("*").WithReducingBy(Mul).ToSelf();
        state.TransitsBy("/").WithReducingBy(Div).ToSelf();
        
        state.TransitsBy("=").WithReducingBy(Eq).ToSelf();
        state.TransitsBy("<>").WithReducingBy(NotEq).ToSelf();
        state.TransitsBy("<").WithReducingBy(Less).ToSelf();
        state.TransitsBy("<=").WithReducingBy(LessOrEq).ToSelf();
        state.TransitsBy(">").WithReducingBy(Greater).ToSelf();
        state.TransitsBy(">=").WithReducingBy(GreaterOrEq).ToSelf();
        
        state.TransitsBy("DUP").WithReducingBy(Dup).ToSelf();
    }

    private static void AddIfElseThen(State<string, MachineState> state, AutomatonBuilder<string, MachineState> builder)
    {
        var noOpState = builder.CreateState();
        noOpState.TransitsBy("ELSE").Dynamicly().WithReducingBy(Else(state.Id));
        noOpState.TransitsBy("THEN").WithReducingBy(Then).To(state);
        noOpState.AllOtherTransits().WithReducingBy(NoOp(noOpState.Id));
        
        state.TransitsBy("IF").Dynamicly().WithReducingBy(If(state.Id, noOpState.Id));
        state.TransitsBy("ELSE").Dynamicly().WithReducingBy(Else(noOpState.Id));
        state.TransitsBy("THEN").WithReducingBy(Then).ToSelf();
    }
}