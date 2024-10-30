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

    public static ReductionResult<string, MachineState> PushFalse(MachineState state, string _) =>
        state.Push(false);

    public static ReductionResult<string, MachineState> PushTrue(MachineState state, string _) =>
        state.Push(true);

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

    public static ReductionResult<string, MachineState> Dup(MachineState state, string _) =>
        state.Pop(out var value).Push(value!).Push(value!);

    public static Reduce<string, MachineState> If(StateId trueStateId, StateId falseStateId)
    {
        return (MachineState machineState, string _) =>
        {
            var nextMachineState = machineState.Pop(out bool condition).PushScope(SyntacticScope.If);
                return new ReductionResult<string, MachineState>(nextMachineState)
                    .DynamiclyGoTo(condition ? trueStateId : falseStateId);
        };
    }
    
    public static ReductionResult<string, MachineState> Then(MachineState state, string _)
    {
        var nextState = state.PopScope(out SyntacticScope scope);
        if (nextState.Error.HasValue)
            return nextState;

        return scope == SyntacticScope.If || scope == SyntacticScope.Else
            ? nextState
            : state.SetError("Unexpected THEN.");
    }

    public static Reduce<string, MachineState> Else(StateId elseStateId)
    {
        return (MachineState state, string _) =>
        {
            var nextState = state.PopScope(out SyntacticScope scope);
            if (nextState.Error.HasValue)
                return nextState;

            return scope == SyntacticScope.If
                ? new ReductionResult<string, MachineState>(nextState.PushScope(SyntacticScope.Else)).DynamiclyGoTo(elseStateId)
                : state.SetError("Unexpected ELSE.");
        };
    }
    
    public static ReductionResult<string, MachineState> PopPrint(MachineState state, string _)
    {
        var nextState = state.Pop(out var value);        
        nextState.Error.OnNone(() => Print(value?.ToString() ?? string.Empty));
        
        return nextState;
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