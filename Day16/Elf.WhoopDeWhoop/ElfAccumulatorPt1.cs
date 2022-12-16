namespace Elf.WhoopDeWhoop;

public readonly ref struct ElfAccumulatorPt1
{
    private readonly string[] lines;

    public ElfAccumulatorPt1(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        Graph graph = new(this.lines);

        return graph.GraphSearch(30, static (item, value) =>
         {
             return Math.Max(item.TotalFlow, value);
         },
         0L);
    }
}