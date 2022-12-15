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
        Span<SignalInfo> sensorPositionsBuffer = stackalloc SignalInfo[lines.Length];
        Span<Point> beaconPositionsBuffer = stackalloc Point[lines.Length];
        int count = 0;

        int minX = int.MaxValue;
        int maxX = 0;
        int maxDeltaX = 0;

        foreach (var line in lines)
        {
            ElfHelpers.ParseLine(line.AsSpan(), out Point sensor, out Point beacon);
            int delta = ElfHelpers.GetDelta(sensor, beacon);
            int deltaY = Math.Abs(sensor.Y - row);
            if (deltaY > delta)
            {
                // Don't add the sensor if it is absolutely out of range
                continue;
            }

            minX = Math.Min(minX, sensor.X);
            maxX = Math.Max(maxX, sensor.X);
            maxDeltaX = Math.Max(Math.Max(0, delta - deltaY), maxDeltaX);
            sensorPositionsBuffer[count] = new(sensor, delta);
            beaconPositionsBuffer[count++] = beacon;
        }

        ReadOnlySpan<SignalInfo> sensorPositions = sensorPositionsBuffer[..count];
        ReadOnlySpan<Point> beaconPositions = beaconPositionsBuffer[..count];

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