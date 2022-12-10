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
    double speedFPS = 10.0;

    Console.CursorVisible = false;

    string[] program = File.ReadAllLines(file.FullName);
    int elapsedSinceTarget = 0;
    bool blank = true;

    while (true)
    {
        var sw = Stopwatch.StartNew();
        ElfAccumulatorCrt accumulator = new(screenBuffer, 40, 6, offset);

        foreach (var line in program)
        {
            accumulator.ProcessLine(line);
        }

        accumulator.DrawScreen(blank);
        
        sw.Stop();
        
        totalTicks += sw.ElapsedTicks;

        frameCounter++;
        elapsedSinceTarget++;

        if (elapsedSinceTarget >= targetFrameCount)
        {
            if (offset < 39)
            {
                offset++;
            }
            else
            {
                offset = 0;
            }

            elapsedSinceTarget = 0;
        }

        if (frameCounter % 50 == 0)
        {
            averageFrameTimeMs = totalTicks / (50 * 10_000.0);
            totalTicks = 0;
            targetFrameCount = (int)Math.Round((1000.0/speedFPS) / averageFrameTimeMs);
            blank = false;
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"FPS: {Math.Round(1000.0 / averageFrameTimeMs)}    ");

        Console.CursorTop = 1;
        Console.CursorTop = 0;
    }
}
