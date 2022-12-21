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
            graph.AddMonkey(line.AsSpan());
        }

        return graph.Process();
    }
}