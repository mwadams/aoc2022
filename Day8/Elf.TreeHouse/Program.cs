using Elf.TreeHouse;
using System.CommandLine;
using System.Diagnostics;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 6 input.");


var rootCommand = new RootCommand("Advent of Code Day 6");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

const double Iterations = 10_000.0;

static void ProcessElfFile(FileInfo file)
{
    ElfAccumulator accumulator = default;


    var sw = Stopwatch.StartNew();
    
    string[] lines = File.ReadAllLines(file.FullName);
    
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator = new(lines);
        accumulator.BuildMetrics();
    }

    sw.Stop();

    Console.WriteLine($"{accumulator.Result}.");
    Console.WriteLine($"Time: {sw.ElapsedMilliseconds / Iterations}ms");


    ElfAccumulatorScenic accumulatorScenic = default;

    var sw2 = Stopwatch.StartNew();

    string[] lines2 = File.ReadAllLines(file.FullName);

    for (int i = 0; i < Iterations; ++i)
    {
        accumulatorScenic = new(lines2);
        accumulatorScenic.BuildMetrics();
    }

    sw2.Stop();

    Console.WriteLine($"{accumulatorScenic.Result}.");
    Console.WriteLine($"Time: {sw2.ElapsedMilliseconds / Iterations}ms");
}