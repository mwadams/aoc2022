namespace Elf.Obsidian;

internal readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public int Process()
    {
        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan());
        }

        return 0;
    }

    private static void ProcessLine(ReadOnlySpan<char> readOnlySpan)
    {
    }
}