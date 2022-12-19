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
        // Only need the first 3 blueprints
        blueprints = blueprints[..3];
        long total = 1;
        int blueprintIndex = 1;
        foreach (var blueprint in blueprints)
        {
            long result = BlueprintAnalyser.AnalyseBlueprint(blueprint, 32);
            total *= result;
            blueprintIndex++;

        }

        return total;
    }
}