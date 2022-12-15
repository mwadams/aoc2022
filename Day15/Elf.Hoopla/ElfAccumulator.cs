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
        Span<SignalInfo> sensorPositions = stackalloc SignalInfo[lines.Length];
        Span<Point> beaconPositions = stackalloc Point[lines.Length];
        int count = 0;

        int minX = int.MaxValue;
        int maxX = 0;
        int maxDeltaX = 0;

        foreach (var line in lines)
        {
            ElfHelpers.ParseLine(line.AsSpan(), out Point sensor, out Point beacon);
            minX = Math.Min(minX, sensor.X);
            maxX = Math.Max(maxX, sensor.X);
            int delta = ElfHelpers.GetDelta(sensor, beacon);
            int deltaY = Math.Abs(sensor.Y - row);
            maxDeltaX = Math.Max(Math.Max(0, delta - deltaY), maxDeltaX);
            sensorPositions[count] = new(sensor, delta);
            beaconPositions[count++] = beacon;
        }

        int result = 0;

        // The maximum range along the row is the max delta either side of the min and max x
        for (int x = minX - maxDeltaX; x <= maxX + maxDeltaX; ++x)
        {
            // Is this out of range of all sensors, and not an existing beacon
            if (!ElfHelpers.IsOutOfRange(x, row, sensorPositions) && !beaconPositions.Contains(new(x, row)))
            {
                result++;
            }
        }

        return result;
    }
}