using Elf.Climbing;
using System.CommandLine;
using System.Diagnostics;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the elf supply stacks.");


var rootCommand = new RootCommand("Advent of Code Day 5");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

const int Iterations = 100;

static void ProcessElfFile(FileInfo file)
{
    string[] lines = File.ReadAllLines(file.FullName);

    ElfAccumulator accumulator = default;

    // Warmup
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator = new(lines);
        accumulator.FindLocation();
    }

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator = new(lines);
        accumulator.FindLocation();
    }

    sw.Stop();

    Console.WriteLine($"{accumulator.ShortestPath}");
    Console.WriteLine($"{sw.ElapsedMilliseconds / (double)Iterations}ms");


    ElfAccumulatorScenic accumulatorScenic = default;

    // Warmup
    for (int i = 0; i < Iterations; ++i)
    {
        accumulatorScenic = new(lines);
        accumulatorScenic.FindLocation();
    }

    sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        accumulatorScenic = new(lines);
        accumulatorScenic.FindLocation();
    }

    sw.Stop();

    Console.WriteLine($"{accumulatorScenic.ShortestPath}");
    Console.WriteLine($"{sw.ElapsedMilliseconds / (double)Iterations}ms");
}