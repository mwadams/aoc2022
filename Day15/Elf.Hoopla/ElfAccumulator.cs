namespace Elf.Hoopla;

public readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public int Process(int row)
    {
        return ProcessLines(this.lines, row);
    }

    private static int ProcessLines(string[] lines, int row)
    {
        Span<Range> rangesBuffer = stackalloc Range[lines.Length];
        Span<int> beaconPositionsBuffer = stackalloc int[lines.Length];
        int rangeCount = 0;
        int beaconCount = 0;
        foreach (var line in lines)
        {
            ElfHelpers.ParseLine(line.AsSpan(), out Point sensor, out Point beacon);
            int delta = ElfHelpers.GetDelta(sensor, beacon);
            int deltaY = Math.Abs(sensor.Y - row);
            int deltaX = delta - deltaY;
            if (beacon.Y == row)
            {
                if (!beaconPositionsBuffer.Contains(beacon.X))
                {
                    beaconPositionsBuffer[beaconCount++] = beacon.X;
                }
            }

            if (deltaX < 0)
            {
                // Don't add the sensor if it is absolutely out of range
                continue;
            }


            rangesBuffer[rangeCount++] = new(sensor.X - deltaX, sensor.X + deltaX);
        }

        Span<Range> ranges = rangesBuffer[..rangeCount];

        ranges.Sort();

        int result = 0;
        int x = int.MinValue;
        foreach (var range in ranges)
        {
            x = Math.Max(x, range.Low);
            if (range.High < x)
            {
                continue;
            }

            int high = range.High + 1;
            result += high - x;
            x = high;
        }

        return result - beaconCount;
    }
}