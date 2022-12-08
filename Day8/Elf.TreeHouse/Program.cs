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

static void ProcessElfFile(FileInfo file)
{
    ElfAccumulator accumulator = default;


    var sw = Stopwatch.StartNew();
    
    string[] lines = File.ReadAllLines(file.FullName);
    
    for (int i = 0; i < 1_000; ++i)
    {
        accumulator = new(lines);
        accumulator.BuildMetrics(true);
    }

    sw.Stop();

    Console.WriteLine($"The visible tree count was {accumulator.VisibleCount}.");
    Console.WriteLine($"The maximum scenic score was {accumulator.MaxScenicScore}.");
    Console.WriteLine($"Time: {sw.ElapsedMilliseconds / 1_000.0}ms");
}