using System.Globalization;
using DFAutomaton;

namespace ForthMachine;

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

        start.AllOtherTransits().WithReducingBy(WordOperations.ExecWord(start.Id));
        
        start.TransitsBy("END").WithReducingBy(NoOperations.NoOp).ToAccepted();
        
        return builder
            .ValidateAnyCanReachAccepted()
            .AddCheckForErrorState(state => state.Error.HasValue)
            .Build()
            .Match(automaton => automaton, error => throw new Exception($"Error: {error}."));
    }

    private static void AddOperations(State<string, MachineState> state)
    {
        state.TransitsWhen(IsNumber).WithReducingBy(StackOperations.PushNumber).ToSelf();
        state.TransitsBy("FALSE").WithReducingBy(StackOperations.PushFalse).ToSelf();
        state.TransitsBy("TRUE").WithReducingBy(StackOperations.PushTrue).ToSelf();
        
        state.TransitsBy(".").WithReducingBy(PrintOperations.PopPrint).ToSelf();
        
        state.TransitsBy("+").WithReducingBy(MathOperations.Add).ToSelf();
        state.TransitsBy("-").WithReducingBy(MathOperations.Sub).ToSelf();
        state.TransitsBy("*").WithReducingBy(MathOperations.Mul).ToSelf();
        state.TransitsBy("/").WithReducingBy(MathOperations.Div).ToSelf();
        state.TransitsBy("NEG").WithReducingBy(MathOperations.Neg).ToSelf();
        state.TransitsBy("ABS").WithReducingBy(MathOperations.Abs).ToSelf();
        
        state.TransitsBy("=").WithReducingBy(CmpOperations.Eq).ToSelf();
        state.TransitsBy("<>").WithReducingBy(CmpOperations.NotEq).ToSelf();
        state.TransitsBy("<").WithReducingBy(CmpOperations.Less).ToSelf();
        state.TransitsBy("<=").WithReducingBy(CmpOperations.LessOrEq).ToSelf();
        state.TransitsBy(">").WithReducingBy(CmpOperations.Greater).ToSelf();
        state.TransitsBy(">=").WithReducingBy(CmpOperations.GreaterOrEq).ToSelf();
        
        state.TransitsBy("DEPTH").WithReducingBy(StackOperations.Depth).ToSelf();
        state.TransitsBy("DUP").WithReducingBy(StackOperations.Dup).ToSelf();
        state.TransitsBy("SWAP").WithReducingBy(StackOperations.Swap).ToSelf();
        state.TransitsBy("OVER").WithReducingBy(StackOperations.Over).ToSelf();
        state.TransitsBy("DROP").WithReducingBy(StackOperations.Drop).ToSelf();
        
        state.TransitsBy("STACK").WithReducingBy(PrintOperations.Stack).ToSelf();
        state.TransitsBy("SCOPE-STACK").WithReducingBy(PrintOperations.ScopeStack).ToSelf();
    }

    private static void AddIfElseThen(State<string, MachineState> state, AutomatonBuilder<string, MachineState> builder)
    {
        var noOpState = builder.CreateState();
        noOpState.TransitsBy("ELSE").Dynamicly().WithReducingBy(IfOperations.Else(state.Id));
        noOpState.TransitsBy("THEN").WithReducingBy(IfOperations.Then).To(state);
        noOpState.AllOtherTransits().WithReducingBy(NoOperations.NoOp(noOpState.Id));
        
        state.TransitsBy("IF").Dynamicly().WithReducingBy(IfOperations.If(state.Id, noOpState.Id));
        state.TransitsBy("ELSE").Dynamicly().WithReducingBy(IfOperations.Else(noOpState.Id));
        state.TransitsBy("THEN").WithReducingBy(IfOperations.Then).ToSelf();
    }

    private static void AddBeginUntil(State<string, MachineState> state)
    {
        var beginState = state.TransitsBy("BEGIN").WithReducingBy(BeginLoopOperations.Begin).ToNew();
        beginState.TransitsBy("UNTIL").WithReducingBy(BeginLoopOperations.BeginFinish).To(state);
        beginState.AllOtherTransits().WithReducingBy(BeginLoopOperations.BeginLoop(beginState.Id));

        state.TransitsBy("UNTIL").WithReducingBy(BeginLoopOperations.Until).ToSelf();
    }

    private static void DefineWord(State<string, MachineState> state)
    {
        var startWordDefinitionState = state.TransitsBy(":").WithReducingBy(WordOperations.StartWordDefinition).ToNew();
        
        var beginWordState = startWordDefinitionState.TransitsWhen(IsNewWord).WithReducingBy(WordOperations.BeginWord).ToNew();
        beginWordState.AllOtherTransits().WithReducingBy(WordOperations.AddInnerWord(beginWordState.Id));
        
        beginWordState.TransitsBy(";").WithReducingBy(WordOperations.FinishWordDefinition).To(state);
    }

    private static bool IsNumber(string word) => decimal.TryParse(word, CultureInfo.InvariantCulture, out var _);

    private static bool IsNewWord(string word) => !IsNumber(word);
}