namespace Elf.Robots;

public readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        Span<Blueprint> blueprints = stackalloc Blueprint[lines.Length];
        int blueprintCount = 0;
        foreach (var line in lines)
        {
            blueprints[blueprintCount++] = BlueprintAnalyser.BuildBlueprint(line.AsSpan());
        }

        return AnalyseBlueprints(blueprints);
    }

    private static long AnalyseBlueprints(ReadOnlySpan<Blueprint> blueprints)
    {
        long total = 0; ;
        int blueprintIndex = 1;
        foreach (var blueprint in blueprints)
        {
            long result = BlueprintAnalyser.AnalyseBlueprint(blueprint, 24);
            total += result * blueprintIndex;
            blueprintIndex++;
        }

        return total;
    }
}