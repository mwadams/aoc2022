namespace Elf.WhoopDeWhoop;

internal readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        Graph graph = new(this.lines);

        long answer2 = 0;
        Dictionary<long, long> bestResults = new();

        graph.GraphSearch(26, static (item, resultSet) =>
        {
            resultSet[item.OpenedValves] =
                  Math.Max(resultSet.TryGetValue(item.OpenedValves, out long v1) ? v1 : 0, item.TotalFlow);
            return resultSet;
        }, bestResults);

        answer2 = FindResultSetsThatDidNotOpenTheSameValves(answer2, bestResults);

        return answer2;
    }

    private static long FindResultSetsThatDidNotOpenTheSameValves(long answer2, Dictionary<long, long> bestResults)
    {
        foreach ((long openMask1, long best1) in bestResults)
        {
            foreach ((long openMask2, long best2) in bestResults)
            {
                long overlap = openMask1 & openMask2;
                if (overlap != 0)
                {
                    continue;
                }

                answer2 = Math.Max(answer2, best1 + best2);
            }
        }

        return answer2;
    }
}