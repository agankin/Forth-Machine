namespace ForthMachine;

public static class MachineStateExtensions
{
    public static MachineState Push(this MachineState state, decimal value) =>
        state.Push(new NumberValue(value));

    public static MachineState Push(this MachineState state, bool value) =>
        state.Push(new BooleanValue(value));

    public static MachineState Pop(this MachineState state, out decimal value)
    {
        decimal valueLocal = 0;
        var nextState = state.Pop(out var machinValue)
            .Map(state =>
            {
                if (machinValue is not NumberValue numberValue)
                    return state.SetError($"{machinValue} is not number.");

                valueLocal = numberValue.Value;
                return state;
            });

        value = valueLocal;
        return nextState;
    }

    public static MachineState Pop(this MachineState state, out bool value)
    {
        bool valueLocal = false;
        var nextState = state.Pop(out var machinValue)
            .Map(state =>
            {
                if (machinValue is not BooleanValue boolValue)
                    return state.SetError($"{machinValue} is not boolean.");

                valueLocal = boolValue.Value;
                return state;
            });

        value = valueLocal;
        return nextState;
    }
}