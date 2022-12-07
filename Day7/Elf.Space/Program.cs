using Elf.Space;
using System.CommandLine;

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
    ElfAccumulator accumulator = new(100_000);

    foreach (var line in File.ReadLines(file.FullName))
    {
        accumulator.ProcessLine(line);
    }

    accumulator.Finish();

    Console.WriteLine($"Total at most MaxSize: {accumulator.TotalAtMostMaxSize}");
    Console.WriteLine($"Directory to delete: {accumulator.GetDirectorySizeToFreeUp(30_000_000, 70_000_000)}");
}