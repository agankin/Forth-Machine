namespace ForthMachine;

public record BooleanValue(bool Value)  : MachineValue()
{
    public override string ToString() => Value.ToString();
}