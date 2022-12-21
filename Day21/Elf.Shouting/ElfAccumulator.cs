namespace Elf.Shouting;

public readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        var graph = new MonkeyGraph();

        foreach (var line in lines)
        {
            graph.ProcessLine(line.AsSpan());
        }

        return graph.Shout();
    }
}