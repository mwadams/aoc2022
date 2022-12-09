using Elf.Shenanigans;
using System.CommandLine;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 9 input.");


var rootCommand = new RootCommand("Advent of Code Day 9");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file)
{
    ElfAccumulator accumulator = new(1, 9);
    foreach (var line in File.ReadLines(file.FullName))
    {
        accumulator.ProcessLine(line);
    }

    Console.WriteLine(accumulator.Visited);
}