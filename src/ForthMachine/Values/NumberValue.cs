using System.Globalization;

namespace ForthMachine;

public record NumberValue(decimal Value) : MachineValue()
{
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}