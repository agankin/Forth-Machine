using PureMonads;

namespace ForthMachine.Repl;

using Words = IEnumerable<string>;
using TrackingResult = Result<SyntacticScopeTracker, string>;

public class ReplReader
{
    private readonly string _finishWord;

    public ReplReader(string finishWord) => _finishWord = finishWord;

    public IEnumerable<Words> ReadInLoop()
    {
        while (true)
        {
            ReplWriter.Prompt();

            var scopeTracker = new SyntacticScopeTracker();
            var words = new List<string>();

            var result = ReadUserInput(scopeTracker, words)
                .OnError(ReplWriter.PrintError);
            
            if (!result.Value().Or(true))
                yield break;

            yield return words;
        }
    }

    private Result<bool, string> ReadUserInput(SyntacticScopeTracker scopeTracker, List<string> words)
    {        
        var userInput = ReadLine();
        if (IsFinishWord(userInput))
            return false;

        var userInputWords = ReplParser.Parse(userInput);
        var trackingResult = TrackScope(scopeTracker, userInputWords);
        words.AddRange(userInputWords);

        return trackingResult.Match(
            scopeTracker => scopeTracker.Scopes.IsEmpty
                ? true
                : ReadUserInput(scopeTracker, words),
            error => error
        );
    }

    private bool IsFinishWord(string userInput) =>
        string.Equals(userInput, _finishWord, StringComparison.OrdinalIgnoreCase);

    private static TrackingResult TrackScope(SyntacticScopeTracker scopeTracker, Words words)
    {
        var initial = TrackingResult.Value(scopeTracker);
        return words.Aggregate(initial, TrackOnNext);
    }

    private static TrackingResult TrackOnNext(TrackingResult result, string word) =>
        result.FlatMap(scopeTracker => scopeTracker.OnNextWord(word));

    private static string ReadLine() => (Console.ReadLine() ?? string.Empty).Trim();
}