using Elf.Malarky;
using System.CommandLine;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 10 input.");


var rootCommand = new RootCommand("Advent of Code Day 10");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file)
{
    ElfAccumulator accumulator = new(20, 40);

    foreach (var line in File.ReadLines(file.FullName))
    {
        accumulator.ProcessLine(line);
    }

    Console.WriteLine($"{accumulator.AccumulatedSignalStrength}");
}