namespace ForthMachine;

public class ReplParser
{
    public static IEnumerable<string> Parse(string userInput)
    {
        var words = userInput.Split().Select(word => word.Trim())
            .Where(word => !string.IsNullOrEmpty(word));

        return words;
    }
}