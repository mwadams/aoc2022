using Elf.Monkeys;
using System.CommandLine;
using System.Diagnostics;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 11 input.");


var rootCommand = new RootCommand("Advent of Code Day 11");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

const int Iterations = 100;

static void ProcessElfFile(FileInfo file)
{
    ElfAccumulator accumulator = default;

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        string[] program = File.ReadAllLines(file.FullName);
        accumulator = new(program);
        accumulator.Play(20, 2, true);
    }
    sw.Stop();

    Console.WriteLine($"{accumulator.MonkeyBusiness}");
    Console.WriteLine($"{sw.ElapsedMilliseconds / (double)Iterations}ms");

    sw = Stopwatch.StartNew();
    for (int i = 0; i < Iterations; ++i)
    {
        string[] program = File.ReadAllLines(file.FullName);
        accumulator = new(program);
        accumulator.Play(10000, 2, false);
    }
    sw.Stop();

    Console.WriteLine($"{accumulator.MonkeyBusiness}");
    Console.WriteLine($"{sw.ElapsedMilliseconds / (double)Iterations}ms");
}
