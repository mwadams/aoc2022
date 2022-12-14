using System.CommandLine;
using Elf.Fuel;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the elf supply stacks.");


var rootCommand = new RootCommand("Advent of Code Day 5");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file)
{
    string[] lines = File.ReadAllLines(file.FullName);

    ElfAccumulator accumulator = new(lines);
    var result = accumulator.Process();
    Console.WriteLine(result);
}