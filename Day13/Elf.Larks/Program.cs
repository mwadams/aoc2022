using Elf.Larks;
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

    ElfAccumulatorPt1 accumulator;
    int result = 0;

    // Warmup
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator = new(lines);
        result = accumulator.ProcessLines();
    }

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator = new(lines);
        result = accumulator.ProcessLines();
    }
    sw.Stop();

    Console.WriteLine(result);
    Console.WriteLine($"{sw.ElapsedMilliseconds / (double)Iterations}ms");


    ElfAccumulatorPt2 accumulator2;
    int result2 = 0;

    // Warmup
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator2 = new(lines);
        result2 = accumulator2.ProcessLines();
    }

    sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        accumulator2 = new(lines);
        result = accumulator2.ProcessLines();
    }
    sw.Stop();

    Console.WriteLine(result2);
    Console.WriteLine($"{sw.ElapsedMilliseconds / (double)Iterations}ms");
}