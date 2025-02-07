using System.Collections.Immutable;
using PureMonads;

namespace ForthMachine;

using TrackingResult = Result<SyntacticScopeTracker, string>;

public readonly record struct SyntacticScopeTracker
{
    public SyntacticScopeTracker()
    {
    }

    public ImmutableStack<SyntacticScope> Scopes { get; init; } = ImmutableStack<SyntacticScope>.Empty;

    public TrackingResult OnNextWord(string word)
    {
        var upperWord = word?.ToUpperInvariant();
        return upperWord switch
        {
            MachineWords.If => Enter(SyntacticScope.If),
            MachineWords.Else => Exit(upperWord, SyntacticScope.If).Pipe(result => Enter(result, SyntacticScope.Else)),
            MachineWords.Then => Exit(upperWord, SyntacticScope.If, SyntacticScope.Else),
            
            MachineWords.Begin => Enter(SyntacticScope.Begin),
            MachineWords.Until => Exit(upperWord, SyntacticScope.Begin),

            MachineWords.Do => Enter(SyntacticScope.Do),
            MachineWords.Loop => Exit(upperWord, SyntacticScope.Do),

            MachineWords.BeginWord => Enter(SyntacticScope.Word),
            MachineWords.EndWord => Exit(upperWord, SyntacticScope.Word),
            
            _ => this
        };
    }

    private SyntacticScopeTracker Enter(SyntacticScope scope) => this with { Scopes = Scopes.Push(scope) };

    private static TrackingResult Enter(TrackingResult result, SyntacticScope scope) =>
        result.Map(tracker => tracker.Enter(scope));

    private TrackingResult Exit(string word, params SyntacticScope[] expectedScopes) =>
        !Scopes.IsEmpty && expectedScopes.Contains(Scopes.Peek())
            ? this with { Scopes = Scopes.Pop(out _) }
            : MachineError.Unexpected(word);
}