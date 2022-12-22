namespace Elf.MonkeyMap;

public readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public int Process()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            ProcessLine(lines, i);
        }

        return 0;
    }

    private void ProcessLine(ReadOnlySpan<string> lines, int index)
    {
    }
}