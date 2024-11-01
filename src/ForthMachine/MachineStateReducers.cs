using System.Globalization;
using DFAutomaton;
using PureMonads;

namespace ForthMachine;

public static class MachineStateReducers
{
    public static ReductionResult<string, MachineState> PushNumber(MachineState state, string numberWord)
    {
        var number = decimal.Parse(numberWord, CultureInfo.InvariantCulture);
        return state.Push(number);
    }

    public static ReductionResult<string, MachineState> PushFalse(MachineState state, string _) => state.Push(false);

    public static ReductionResult<string, MachineState> PushTrue(MachineState state, string _) => state.Push(true);

    public static ReductionResult<string, MachineState> PopPrint(MachineState state, string _)
    {
        var nextState = state.Pop(out var value);        
        nextState.Error.OnNone(() => Print(value?.ToString() ?? string.Empty));
        
        return nextState;
    }
    
    public static ReductionResult<string, MachineState> Add(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => state.Push(number1 + number2));

    public static ReductionResult<string, MachineState> Sub(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => state.Push(number1 - number2));

    public static ReductionResult<string, MachineState> Mul(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => state.Push(number1 * number2));

    public static ReductionResult<string, MachineState> Div(MachineState state, string _) =>
        state.Pop(out decimal number2).Pop(out decimal number1)
            .Map(state => number2 != 0
                ? state.Push(number1 / number2)
                : state.SetError("Division by zero!"));

    public static ReductionResult<string, MachineState> Neg(MachineState state, string _) =>
        state.Pop(out decimal number)
            .Map(state => state.Push(-1 * number));

    public static ReductionResult<string, MachineState> Abs(MachineState state, string _) =>
        state.Pop(out decimal number)
            .Map(state => state.Push(Math.Abs(number)));

    public static ReductionResult<string, MachineState> Eq(MachineState state, string _) =>
        state.Pop(out var value2).Pop(out var value1)
            .Map(state => value1?.GetType() == value2?.GetType()
                ? state
                : state.SetError($"Compared values {value2} and {value1} have different types."))
            .Map(state => state.Push(value1 == value2));

    public static ReductionResult<string, MachineState> NotEq(MachineState state, string _) =>
        state.Pop(out var value2).Pop(out var value1)
            .Map(state => value1?.GetType() == value2?.GetType()
                ? state
                : state.SetError($"Compared values {value2} and {value1} have different types."))
            .Map(state => state.Push(value1 != value2));

    public static ReductionResult<string, MachineState> Less(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 < value2));
    
    public static ReductionResult<string, MachineState> LessOrEq(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 <= value2));

    public static ReductionResult<string, MachineState> Greater(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 > value2));
    
    public static ReductionResult<string, MachineState> GreaterOrEq(MachineState state, string _) =>
        state.Pop(out decimal value2).Pop(out decimal value1)
            .Map(state => state.Push(value1 >= value2));

    public static ReductionResult<string, MachineState> Depth(MachineState state, string _) =>
        state.Push(state.Stack.Count());
    
    public static ReductionResult<string, MachineState> Dup(MachineState state, string _) =>
        state.Pop(out var value).Push(value!).Push(value!);

    public static ReductionResult<string, MachineState> Swap(MachineState state, string _) =>
        state.Pop(out var value1).Pop(out var value2).Push(value1!).Push(value2!);

    public static ReductionResult<string, MachineState> Over(MachineState state, string _) =>
        state.Pop(out var value1).Pop(out var value2).Push(value2!).Push(value1!).Push(value2!);

    public static ReductionResult<string, MachineState> Drop(MachineState state, string _) =>
        state.Pop(out var _);

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

    public static Reduce<string, MachineState> If(StateId trueStateId, StateId falseStateId)
    {
        return (MachineState machineState, string _) =>
        {
            var nextMachineState = machineState.Pop(out bool condition).PushScope(new IfSyntacticScope());
                return new ReductionResult<string, MachineState>(nextMachineState)
                    .DynamiclyGoTo(condition ? trueStateId : falseStateId);
        };
    }
    
    public static ReductionResult<string, MachineState> Then(MachineState state, string _)
    {
        var nextState = state.PopScope(out SyntacticScope scope);

        return scope is IfSyntacticScope || scope is ElseSyntacticScope
            ? nextState
            : state.SetError("Unexpected THEN.");
    }

    public static Reduce<string, MachineState> Else(StateId elseStateId)
    {
        return (MachineState state, string _) =>
        {
            var nextState = state.PopScope(out SyntacticScope scope);

            return scope is IfSyntacticScope
                ? new ReductionResult<string, MachineState>(nextState.PushScope(new ElseSyntacticScope()))
                    .DynamiclyGoTo(elseStateId)
                : state.SetError("Unexpected ELSE.");
        };
    }

    public static ReductionResult<string, MachineState> Begin(MachineState state, string _) =>
        state.PushScope(BeginSyntacticScope.Empty);

    public static Reduce<string, MachineState> BeginLoop(StateId gotoStateId)
    {
        return (MachineState state, string word) =>
        {
            var state2 = state.PopScope(out SyntacticScope scope);
            var beginScope = (BeginSyntacticScope)scope;
            
            var state3 = state2.PushScope(beginScope.AddWord(word));

            return new ReductionResult<string, MachineState>(state3)
                .DynamiclyGoTo(gotoStateId);
        };
    }

    public static ReductionResult<string, MachineState> BeginFinish(MachineState state, string _)
    {
        var state2 = state.PopScope(out SyntacticScope scope);
        var beginScope = (BeginSyntacticScope)scope;
        
        var result = new ReductionResult<string, MachineState>(state2.PushScope(beginScope));
        foreach (var word in beginScope.LoopWords.Append("UNTIL"))
            result.YieldNext(word);
        
        return result;
    }

    public static ReductionResult<string, MachineState> Until(MachineState state, string _)
    {
        var state2 = state.PopScope(out SyntacticScope scope);
        if (scope is not BeginSyntacticScope beginScope)
            return state2.SetError("Unexpected UNTIL.");

        var state3 = state2.Pop(out bool condition);
        if (state3.Error.HasValue)
            return state3;

        if (condition)
            return state3;

        var result = new ReductionResult<string, MachineState>(state3.PushScope(beginScope));
        foreach (var word in beginScope.LoopWords.Append("UNTIL"))
            result.YieldNext(word);

        return result;
    }

    public static ReductionResult<string, MachineState> NoOp(MachineState state, string _) => state;

    public static Reduce<string, MachineState> NoOp(StateId goToStateId) =>
        (state, _) => new ReductionResult<string, MachineState>(state)
            .DynamiclyGoTo(goToStateId);

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