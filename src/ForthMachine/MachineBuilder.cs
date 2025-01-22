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
        AddIf(start, builder);
        AddBeginLoop(start);
        AddDoLoop(start);
        AddWordDefinition(start);

        start.AllOtherTransits().WithReducingBy(DefineWordOperations.ExecWord).ToSelf();
        
        start.TransitsBy("END").WithReducingBy(NoOperations.NoOp).ToAccepted();
        
        return builder
            .ValidateAnyCanReachAccepted()
            .AddCheckForErrorState(state => !state.Valid())
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
        
        state.TransitsBy("=").WithReducingBy(CompareOperations.Eq).ToSelf();
        state.TransitsBy("<>").WithReducingBy(CompareOperations.NotEq).ToSelf();
        state.TransitsBy("<").WithReducingBy(CompareOperations.Less).ToSelf();
        state.TransitsBy("<=").WithReducingBy(CompareOperations.LessOrEq).ToSelf();
        state.TransitsBy(">").WithReducingBy(CompareOperations.Greater).ToSelf();
        state.TransitsBy(">=").WithReducingBy(CompareOperations.GreaterOrEq).ToSelf();
        
        state.TransitsBy("DEPTH").WithReducingBy(StackOperations.Depth).ToSelf();
        state.TransitsBy("DUP").WithReducingBy(StackOperations.Dup).ToSelf();
        state.TransitsBy("SWAP").WithReducingBy(StackOperations.Swap).ToSelf();
        state.TransitsBy("OVER").WithReducingBy(StackOperations.Over).ToSelf();
        state.TransitsBy("DROP").WithReducingBy(StackOperations.Drop).ToSelf();
        
        state.TransitsBy("STACK").WithReducingBy(PrintOperations.Stack).ToSelf();
        state.TransitsBy("SCOPE-STACK").WithReducingBy(PrintOperations.ScopeStack).ToSelf();
    }

    private static void AddIf(State<string, MachineState> state, AutomatonBuilder<string, MachineState> builder)
    {
        var noOpState = builder.CreateState();
        noOpState.TransitsBy(MachineWords.Else).Dynamicly().WithReducingBy(IfOperations.BeginElse(state.Id));
        noOpState.TransitsBy(MachineWords.Then).WithReducingBy(IfOperations.EndIf).To(state);
        noOpState.AllOtherTransits().WithReducingBy(NoOperations.NoOp).ToSelf();
        
        state.TransitsBy(MachineWords.If).Dynamicly().WithReducingBy(IfOperations.BeginIf(state.Id, noOpState.Id));
        state.TransitsBy(MachineWords.Else).Dynamicly().WithReducingBy(IfOperations.BeginElse(noOpState.Id));
        state.TransitsBy(MachineWords.Then).WithReducingBy(IfOperations.EndIf).ToSelf();
    }

    private static void AddBeginLoop(State<string, MachineState> state)
    {
        var beginState = state.TransitsBy(MachineWords.Begin).WithReducingBy(BeginLoopOperations.BeginLoop).ToNew();
        beginState.TransitsBy(MachineWords.Until).WithReducingBy(BeginLoopOperations.EndLoop).To(state);
        beginState.AllOtherTransits().WithReducingBy(BeginLoopOperations.AddInnerWord).ToSelf();

        state.TransitsBy(MachineWords.Until).WithReducingBy(BeginLoopOperations.Repeat).ToSelf();
    }

    private static void AddDoLoop(State<string, MachineState> state)
    {
        var beginState = state.TransitsBy(MachineWords.Do).WithReducingBy(DoLoopOperations.BeginLoop).ToNew();
        beginState.TransitsBy(MachineWords.Loop).WithReducingBy(DoLoopOperations.EndLoop).To(state);
        beginState.AllOtherTransits().WithReducingBy(DoLoopOperations.AddInnerWord).ToSelf();

        state.TransitsBy(MachineWords.Loop).WithReducingBy(DoLoopOperations.Repeat).ToSelf();
    }

    private static void AddWordDefinition(State<string, MachineState> state)
    {
        var startWordDefinitionState = state.TransitsBy(MachineWords.BeginWord)
            .WithReducingBy(DefineWordOperations.BeginWordDefinition)
            .ToNew();
        
        var beginWordState = startWordDefinitionState.TransitsWhen(IsNewWord).WithReducingBy(DefineWordOperations.SetWord).ToNew();
        beginWordState.AllOtherTransits().WithReducingBy(DefineWordOperations.AddInnerWord).ToSelf();
        
        beginWordState.TransitsBy(MachineWords.EndWord)
            .WithReducingBy(DefineWordOperations.EndWordDefinition)
            .To(state);
    }

    private static bool IsNumber(string word) =>
        decimal.TryParse(word, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var _);

    private static bool IsNewWord(string word) => !IsNumber(word);
}