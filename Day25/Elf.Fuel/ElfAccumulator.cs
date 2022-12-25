namespace Elf.Fuel;

public readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public string Process()
    {
        long total = 0;
        foreach(var line in lines)
        {
            total += ProcessLine(line.AsSpan());
        }

        return NumberSystem.BuildSnafu(total);
    }

    private static long ProcessLine(ReadOnlySpan<char> line)
    {
        return NumberSystem.GetNumber(line);
    }
}