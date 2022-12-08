using Elf.TreeHouse;
using System.CommandLine;

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
    ElfAccumulator accumulator = new(File.ReadAllLines(file.FullName));
    accumulator.BuildMetrics();
    Console.WriteLine($"The number of visible trees was {accumulator.VisibleCount}.");
    Console.WriteLine($"The maximum scenic score was {accumulator.MaxScenicScore}.");
}