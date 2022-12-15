using Elf.Hoopla;
using System.CommandLine;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 15 instructions.");

var pt1Option = new Option<int>(
    name: "--pt1",
    description: "The row to test for part 1");

var pt2Option = new Option<int>(
    name: "--pt2",
    description: "The range to test for part 2");


var rootCommand = new RootCommand("Advent of Code Day 15");
rootCommand.AddOption(fileOption);
rootCommand.AddOption(pt1Option);
rootCommand.AddOption(pt2Option);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption,
    pt1Option,
    pt2Option);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file, int pt1Option, int pt2Option)
{
    string[] lines = File.ReadAllLines(file.FullName);

    ElfAccumulator accumulator = new(lines);
    var result = accumulator.Process(pt1Option);
    Console.WriteLine(result);


    ElfAccumulatorPt2 accumulator2 = new(lines);
    var result2 = accumulator2.Process(pt2Option);
    Console.WriteLine(result2);
}