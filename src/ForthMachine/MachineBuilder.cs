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
        AddBeginUntil(start);
        DefineWord(start);

        start.AllOtherTransits().WithReducingBy(ExecWord(start.Id));
        
        start.TransitsBy("END").WithReducingBy(NoOp).ToAccepted();
        
        return builder
            .ValidateAnyCanReachAccepted()
            .AddCheckForErrorState(state => state.Error.HasValue)
            .Build()
            .Match(automaton => automaton, error => throw new Exception($"Error: {error}."));
    }

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

        state.TransitsBy("NEG").WithReducingBy(Neg).ToSelf();
        state.TransitsBy("ABS").WithReducingBy(Abs).ToSelf();
        
        state.TransitsBy("=").WithReducingBy(Eq).ToSelf();
        state.TransitsBy("<>").WithReducingBy(NotEq).ToSelf();
        state.TransitsBy("<").WithReducingBy(Less).ToSelf();
        state.TransitsBy("<=").WithReducingBy(LessOrEq).ToSelf();
        state.TransitsBy(">").WithReducingBy(Greater).ToSelf();
        state.TransitsBy(">=").WithReducingBy(GreaterOrEq).ToSelf();
        
        state.TransitsBy("DEPTH").WithReducingBy(Depth).ToSelf();
        state.TransitsBy("DUP").WithReducingBy(Dup).ToSelf();
        state.TransitsBy("SWAP").WithReducingBy(Swap).ToSelf();
        state.TransitsBy("OVER").WithReducingBy(Over).ToSelf();
        state.TransitsBy("DROP").WithReducingBy(Drop).ToSelf();
        
        state.TransitsBy("STACK").WithReducingBy(Stack).ToSelf();
        state.TransitsBy("SCOPE-STACK").WithReducingBy(ScopeStack).ToSelf();
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

    private static void AddBeginUntil(State<string, MachineState> state)
    {
        var beginState = state.TransitsBy("BEGIN").WithReducingBy(Begin).ToNew();
        beginState.TransitsBy("UNTIL").WithReducingBy(BeginFinish).To(state);
        beginState.AllOtherTransits().WithReducingBy(BeginLoop(beginState.Id));

        state.TransitsBy("UNTIL").WithReducingBy(Until).ToSelf();
    }

    private static void DefineWord(State<string, MachineState> state)
    {
        var startWordDefinitionState = state.TransitsBy(":").WithReducingBy(StartWordDefinition).ToNew();
        
        var beginWordState = startWordDefinitionState.TransitsWhen(IsNewWord).WithReducingBy(BeginWord).ToNew();
        beginWordState.AllOtherTransits().WithReducingBy(AddInnerWord(beginWordState.Id));
        
        beginWordState.TransitsBy(";").WithReducingBy(FinishWordDefinition).To(state);
    }

    private static bool IsNumber(string word) => decimal.TryParse(word, CultureInfo.InvariantCulture, out var _);

    private static bool IsNewWord(string word) => !IsNumber(word);
}