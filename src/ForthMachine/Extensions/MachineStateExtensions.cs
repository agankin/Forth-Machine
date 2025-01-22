namespace ForthMachine;

internal static class MachineStateExtensions
{
    public static MachineState Push(this MachineState state, decimal value) =>
        state.Push(new NumberValue(value));

    public static MachineState Push(this MachineState state, bool value) =>
        state.Push(new BooleanValue(value));

    public static MachineState Pop(this MachineState state, out decimal value)
    {
        decimal valueLocal = 0;
        var nextState = state.Pop(out var machineValue)
            .Map(state =>
            {
                if (machineValue is not NumberValue numberValue)
                    return state.SetError($"{machineValue} is not number.");

                valueLocal = numberValue.Value;
                return state;
            });

        value = valueLocal;
        return nextState;
    }

    public static MachineState Pop(this MachineState state, out bool value)
    {
        bool valueLocal = false;
        var nextState = state.Pop(out var machineValue)
            .Map(state =>
            {
                if (machineValue is not BooleanValue boolValue)
                    return state.SetError($"{machineValue} is not boolean.");

                valueLocal = boolValue.Value;
                return state;
            });

        value = valueLocal;
        return nextState;
    }

    public static MachineState PopScope<TScope>(this MachineState state, out TScope scope)
        where TScope : ScopeState
    {
        var nextState = state.PopScope(out ScopeState value);
        scope = (TScope)value;

        return nextState;
    }

    public static bool Valid(this MachineState machineState) => !machineState.Error.HasValue;
}