namespace Elf.MonkeyMap;

public readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        MonkeyWalk.BuildMap(lines, out ReadOnlySpan<char> instructions, out int mapHeight);
        return MonkeyWalk.GoForAWalk(lines.AsSpan()[0..mapHeight], instructions);
    }
}