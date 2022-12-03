using Elf.RockPaperScissors;
using System.CommandLine;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the elf Rock Paper Scissors instructions.");


var rootCommand = new RootCommand("Advent of Code Day 1 Calorimeter");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file)
{
    ElfAccumulatorWithStrategy accumulator = new();

    // We could switch this to read lines of char/utf8byte out of the buffer and avoid
    // allocating all the strings, and process a ReadOnlySpan<char> or ReadOnlySpan<byte> (depending on the encoding)
    foreach (var line in File.ReadLines(file.FullName))
    {
        accumulator.ProcessLine(line);
    }

    Console.WriteLine($"The total score was {accumulator.TotalScore}.");
}