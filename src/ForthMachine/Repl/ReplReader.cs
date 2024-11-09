namespace ForthMachine;

public static class ReplReader
{
    public static IEnumerable<string> ReadInLoop(string finishPhrase)
    {
        Console.Write("> ");
        while (ReadUserInput(finishPhrase, out var userInput))
        {
            yield return userInput;

            Console.Write("> ");
        }
    }

    private static bool ReadUserInput(string finishPhrase, out string userInput)
    {
        userInput = Console.ReadLine() ?? string.Empty;
        return !string.Equals(userInput.Trim(), finishPhrase, StringComparison.OrdinalIgnoreCase);
    }
}