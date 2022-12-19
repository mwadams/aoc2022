namespace Elf.Robots;


internal readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan());
        }

        return 0;
    }

    private void ProcessLine(ReadOnlySpan<char> readOnlySpan)
    {
    }
}