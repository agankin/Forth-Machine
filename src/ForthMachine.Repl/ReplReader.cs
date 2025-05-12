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

            var replInputScopeTracker = new SyntacticScopeTracker();
            var words = new List<string>();

            var result = ReadUserInput(replInputScopeTracker, words)
                .OnError(ReplWriter.PrintError);

            if (!result.HasValue)
                continue;
            
            if (!result.Value().Or(true))
                yield break;

            yield return words;
        }
    }

    private Result<bool, string> ReadUserInput(SyntacticScopeTracker replInputScopeTracker, List<string> words)
    {        
        var userInput = ReadLine();
        if (IsFinishWord(userInput))
            return false;

        var userInputWords = ReplParser.Parse(userInput);
        var trackingResult = TrackScope(replInputScopeTracker, userInputWords);
        words.AddRange(userInputWords);

        return trackingResult.Match(
            replInputScopeTracker => replInputScopeTracker.HasInnerScope
                ? ReadUserInput(replInputScopeTracker, words)
                : true,
            error => error
        );
    }

    private bool IsFinishWord(string userInput) =>
        string.Equals(userInput, _finishWord, StringComparison.OrdinalIgnoreCase);

    private static TrackingResult TrackScope(SyntacticScopeTracker scopeTracker, Words words)
    {
        var initial = TrackingResult.Value(scopeTracker);
        return words.Aggregate(initial, TrackNext);
    }

    private static TrackingResult TrackNext(TrackingResult result, string word) =>
        result.FlatMap(scopeTracker => scopeTracker.TrackNext(word));

    private static string ReadLine() => (Console.ReadLine() ?? string.Empty).Trim();
}