namespace Elf.Shouting;

public readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        MonkeyGraph2 graph = new();
        foreach (var line in lines)
        {
            graph.ProcessLine(line.AsSpan());
        }

        return graph.Shout();
    }
}