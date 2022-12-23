namespace Elf.MonkeyMap;

public readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        MonkeyWalk2.BuildMap(lines, out ReadOnlySpan<char> instructions, out int mapHeight, out int mapWidth);
        return MonkeyWalk2.GoForAWalk(lines.AsSpan()[0..mapHeight], instructions, mapWidth, mapHeight);
    }
}