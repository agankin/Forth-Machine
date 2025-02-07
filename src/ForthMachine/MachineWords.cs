using System.Globalization;

namespace ForthMachine;

internal static class MachineWords
{
    public const string If = "IF";

    public const string Else = "ELSE";

    public const string Then = "THEN";

    public const string Begin = "BEGIN";

    public const string Until = "UNTIL";

    public const string Do = "DO";

    public const string Loop = "LOOP";

    public const string BeginWord = ":";

    public const string EndWord = ";";

    public static bool IsNumber(string word) =>
        decimal.TryParse(word, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var _);

    public static bool IsNewWord(string word) => !IsNumber(word);
}