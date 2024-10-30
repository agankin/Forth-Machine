using System.Globalization;

namespace ForthMachine;

public abstract record MachineValue();

public record NumberValue(decimal Value) : MachineValue()
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}

public record BooleanValue(bool Value)  : MachineValue()
{
    public override string ToString() => Value.ToString();
}