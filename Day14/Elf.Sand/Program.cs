using Elf.Sand;
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

const int Iterations = 1000;

static void ProcessElfFile(FileInfo file)
{
    string[] lines = File.ReadAllLines(file.FullName);

    ElfAccumulator accumulator = default;
    int units = 0;

    // Warmup
    for (int i = 0; i < Iterations / 100; ++i)
    {
        accumulator = new(lines);
        units = accumulator.FindUnits(new Point(500, 0));
    }

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations / 10; ++i)
    {
        accumulator = new(lines);
        units = accumulator.FindUnits(new Point(500, 0));
    }
    sw.Stop();

    Console.WriteLine(units);
    Console.WriteLine($"Pt1: {sw.ElapsedMilliseconds / (Iterations / 10.0)}ms");

    ElfAccumulatorPt2 accumulator2;
    int units2 = 0;

    // Warmup
    for (int i = 0; i < Iterations / 100; ++i)
    {
        accumulator2 = new(lines);
        units2 = accumulator2.FindUnits(new Point(500, 0));
    }

    sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator2 = new(lines);
        units2 = accumulator2.FindUnits(new Point(500, 0));
    }
    sw.Stop();

    Console.WriteLine(units2);
    Console.WriteLine($"Pt2: {sw.ElapsedMilliseconds / (double)Iterations}ms");

}