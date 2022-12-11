using Elf.Monkeys;
using System.CommandLine;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 11 input.");


var rootCommand = new RootCommand("Advent of Code Day 11");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file)
{
    string[] program = File.ReadAllLines(file.FullName);
    ElfAccumulator accumulator = new(program);

    accumulator.Play(10000, 2, false);

    Console.WriteLine($"{accumulator.MonkeyBusiness}");
}
