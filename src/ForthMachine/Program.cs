using System.Diagnostics;
using ForthMachine;
using PureMonads;

var machine = MachineBuilder.Build();
var initialState = MachineState.Initial;

ReplReader.ReadInLoop("bye")
    .Select(ReplParser.Parse)
    .Aggregate(initialState, Run);

MachineState Run(MachineState state, IEnumerable<string> words)
{
    if (!words.Any())
        return state;

    var stopwatch = new Stopwatch();
    stopwatch.Start();

    var result = machine.Run(state, words.Append("END"))
        .OnError(ReplWriter.PrintError);

    stopwatch.Stop();
    ReplWriter.PrintOk(stopwatch.ElapsedMilliseconds);

    return result.Value().Or(state);
}