using Elf.Shenanigans;
using System.CommandLine;
using System.Diagnostics;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 9 input.");


var rootCommand = new RootCommand("Advent of Code Day 9");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

const double Iterations = 100.0;

static void ProcessElfFile(FileInfo file)
{
    ElfAccumulator accumulator = default;

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator = new(1, 1);
        foreach (var line in File.ReadLines(file.FullName))
        {
            accumulator.ProcessLine(line);
        }

    }
    sw.Stop();

    Console.WriteLine(accumulator.Visited);
    Console.WriteLine($"{sw.ElapsedMilliseconds / Iterations}ms");

    sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator = new(1, 9);
        foreach (var line in File.ReadLines(file.FullName))
        {
            accumulator.ProcessLine(line);
        }

    }
    sw.Stop();

    Console.WriteLine(accumulator.Visited);
    Console.WriteLine($"{sw.ElapsedMilliseconds / Iterations}ms");
}