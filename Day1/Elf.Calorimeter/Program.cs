using System.CommandLine;

var fileOption = new Option<FileInfo>(
    name: "--file",
    description: "The file containing the elf calories.");

var topCount = new Option<int>(
    name: "--topCount",
    description: "The number of elves to list in the top count.");

var rootCommand = new RootCommand("Advent of Code Day 1 Calorimeter");
rootCommand.AddOption(fileOption);
rootCommand.AddOption(topCount);

rootCommand.SetHandler(
    ProcessElfFile,
    fileOption,
    topCount);

return await rootCommand.InvokeAsync(args);

static void ProcessElfFile(FileInfo file, int topCount)
{
    Span<Elf> elves = stackalloc Elf[topCount];
    ElfAccumulator accumulator = new(ref elves);

    // We could switch this to read lines of char/utf8byte out of the buffer and avoid
    // allocating all the strings, and process a ReadOnlySpan<char> or ReadOnlySpan<byte> (depending on the encoding)
    foreach (var line in File.ReadLines(file.FullName))
    {
        accumulator.ProcessLine(line);
    }

    accumulator.FinishProcessing();

    Console.WriteLine($"The top {topCount} elves are carrying {TotalCalories(elves)} calories.");

}

/// <summary>
/// Calcuates the total calorie count carried by a span of elves.
/// </summary>
static int TotalCalories(in Span<Elf> elves)
{
    int total = 0;
    foreach (var elf in elves)
    {
        if (!elf.IsEmpty) // Belt and braces check; the calorie count will actually be zero for empty records
        {
            total += elf.CalorieCount;
        }
    }

    return total;
}
