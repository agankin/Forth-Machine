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

        start.AllOtherTransits()
            .WithReducingBy(DefineWordOperations.CallWord)
            .ToSelf();
        
        start.TransitsBy("END")
            .WithReducingBy(NoOperations.NoOp)
            .ToAccepted();
        
        return builder
            .ValidateAnyCanReachAccepted()
            .AddCheckForErrorState(state => !state.Valid())
            .Build()
            .Match(automaton => automaton, error => throw new Exception($"Error: {error}."));
    }

    private static void AddOperations(State<string, MachineState> state)
    {
        state.TransitsWhen(MachineWords.IsNumber).WithReducingBy(StackOperations.PushNumber).ToSelf();
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
        var skipBranchState = builder.CreateState();
        skipBranchState.TransitsBy(MachineWords.Else).Dynamicly().WithReducingBy(IfOperations.Else(state.Id));
        skipBranchState.TransitsBy(MachineWords.Then).WithReducingBy(IfOperations.Then).To(state);
        skipBranchState.AllOtherTransits().WithReducingBy(NoOperations.NoOp).ToSelf();
        
        state.TransitsBy(MachineWords.If).Dynamicly()
            .WithReducingBy(IfOperations.If(state.Id, skipBranchState.Id));
        state.TransitsBy(MachineWords.Else).Dynamicly()
            .WithReducingBy(IfOperations.Else(skipBranchState.Id));
        state.TransitsBy(MachineWords.Then)
            .WithReducingBy(IfOperations.Then)
            .ToSelf();
    }

    private static void AddBeginLoop(State<string, MachineState> state)
    {
        var processBodyState = state.TransitsBy(MachineWords.Begin)
            .WithReducingBy(BeginLoopOperations.Begin)
            .ToNew();
        
        processBodyState.TransitsBy(MachineWords.Until)
            .WithReducingBy(BeginLoopOperations.Until)
            .To(state);
        
        processBodyState.AllOtherTransits()
            .WithReducingBy(BeginLoopOperations.ProcessBody)
            .ToSelf();

        state.TransitsBy(MachineWords.Until).WithReducingBy(BeginLoopOperations.Repeat).ToSelf();
    }

    private static void AddDoLoop(State<string, MachineState> state)
    {
        var processBodyState = state.TransitsBy(MachineWords.Do)
            .WithReducingBy(DoLoopOperations.Do)
            .ToNew();
        
        processBodyState.TransitsBy(MachineWords.Loop)
            .WithReducingBy(DoLoopOperations.Loop)
            .To(state);
        
        processBodyState.AllOtherTransits()
            .WithReducingBy(DoLoopOperations.ProcessBody)
            .ToSelf();

        state.TransitsBy(MachineWords.Loop).WithReducingBy(DoLoopOperations.Repeat).ToSelf();
    }

    private static void AddWordDefinition(State<string, MachineState> state)
    {
        var awaitNameState = state.TransitsBy(MachineWords.BeginWord)
            .WithReducingBy(DefineWordOperations.BeginWord)
            .ToNew();
        
        var processBodyState = awaitNameState.TransitsWhen(MachineWords.IsNewWord)
            .WithReducingBy(DefineWordOperations.SetName)
            .ToNew();
        
        processBodyState.AllOtherTransits()
            .WithReducingBy(DefineWordOperations.ProcessBody)
            .ToSelf();
        
        processBodyState.TransitsBy(MachineWords.EndWord)
            .WithReducingBy(DefineWordOperations.EndWord)
            .To(state);
    }
}