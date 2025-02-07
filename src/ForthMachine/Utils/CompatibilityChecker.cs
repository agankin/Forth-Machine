using PureMonads;

namespace ForthMachine;

internal static class CompatibilityChecker
{
    public static Option<string> Check(MachineValue value1, MachineValue value2)
    {
        return value1.GetType() == value2.GetType()
            ? Option.None<string>()
            : MachineError.IncompatibleTypes(value1, value2);
    }
}