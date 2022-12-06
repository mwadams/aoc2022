using Elf.Tuning;
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
    foreach (var line in File.ReadLines(file.FullName))
    {
        ElfAccumulator accumulator = new(14);
        accumulator.ProcessLine(line);
        Console.WriteLine($"The number of characters read was {accumulator.CharactersRead}.");
    }
}