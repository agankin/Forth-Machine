namespace ForthMachine;

internal static class MachineError
{
    public const string StackIsEmpty = "Stack is empty.";

    public const string DivisionByZero = "Division by zero.";

    public static string Unexpected(string word) => $"Unexpected {word} word.";

    public static string IncompatibleTypes(MachineValue value1, MachineValue value2) =>
        $"Values {value1} and {value2} have incompatible types.";
}