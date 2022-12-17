using Elf.Tetris;
using System.CommandLine;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the Day 17 instructions.");


var rootCommand = new RootCommand("Advent of Code Day 1u7");
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


    ElfAccumulatorPt2 accumulator2 = new(lines);
    var result2 = accumulator2.Process();
    Console.WriteLine(result2);
}