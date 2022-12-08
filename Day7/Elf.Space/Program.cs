using Elf.Space;
using System.CommandLine;
using System.Diagnostics;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 7 input.");


var rootCommand = new RootCommand("Advent of Code Day 7");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file)
{
    int totalSize = 0;
    int directorySize = 0;

    var sw = Stopwatch.StartNew();

    for (int i = 0; i < 10_000; ++i)
    {
        ElfAccumulator accumulator = new(100_000);

        foreach (var line in File.ReadLines(file.FullName))
        {
            accumulator.ProcessLine(line);
        }

        accumulator.Finish();

        totalSize = accumulator.TotalAtMostMaxSize;
        directorySize = accumulator.GetDirectorySizeToFreeUp(30_000_000, 70_000_000);
    }

    sw.Stop();

    Console.WriteLine($"Total at most MaxSize: {totalSize}");
    Console.WriteLine($"Directory to delete: {directorySize}");
    Console.WriteLine($"Total time: {sw.ElapsedMilliseconds / 10_000.0}ns");
}