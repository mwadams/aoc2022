using Elf.Malarky;
using System.CommandLine;
using System.Diagnostics;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the day 10 input.");


var rootCommand = new RootCommand("Advent of Code Day 10");
rootCommand.AddOption(fileOption);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file)
{
    int offset = 0;
    int frameCounter = 0;
    long totalTicks = 0;
    double averageFrameTimeMs = 6.25;
    int targetFrameCount = (int)Math.Round((100.0 / averageFrameTimeMs));
    Span<char> screenBuffer = stackalloc char[40 * 6];

    Console.CursorVisible = false;

    string[] program = File.ReadAllLines(file.FullName);

    while (true)
    {
        var sw = Stopwatch.StartNew();
        ElfAccumulatorCrt accumulator = new(screenBuffer, 40, 6, offset);

        foreach (var line in program)
        {
            accumulator.ProcessLine(line);
        }

        accumulator.DrawScreen();
        
        sw.Stop();
        
        totalTicks += sw.ElapsedTicks;

        frameCounter++;

        if (frameCounter % 1000 == 0)
        {
            averageFrameTimeMs = totalTicks / (1000 * 10_000.0);
            totalTicks = 0;
            targetFrameCount = (int)Math.Round((100.0 / averageFrameTimeMs));
        }

        if (frameCounter % targetFrameCount == 0)
        {
            if (offset < 39)
            {
                offset++;
            }
            else
            {
                offset = 0;
            }
        }


        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"FPS: {Math.Round(1000.0 / averageFrameTimeMs)}    ");

        Console.CursorTop = 1;
        Console.CursorTop = 0;
    }
}
