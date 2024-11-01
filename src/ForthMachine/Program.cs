using System.Diagnostics;
using DFAutomaton;
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
        .OnError(PrintError);

    stopwatch.Stop();
    PrintExecTime(stopwatch.ElapsedMilliseconds);

    return result.Value().Or(state);
}

void PrintError(AutomatonError<string, MachineState> error)
{
    var color = Console.ForegroundColor;
    try
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error.Format());
    }
    finally
    {
        Console.ForegroundColor = color;
    }
}

void PrintExecTime(long milliseconds)
{
    Console.WriteLine($"Exec time: {milliseconds}ms.");
}