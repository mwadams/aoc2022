﻿namespace Elf.Hoopla;

internal readonly ref struct ElfAccumulator
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
        int maxDelta = 0;

        foreach (var line in lines)
        {
            ElfHelpers.ParseLine(line.AsSpan(), out Point sensor, out Point beacon);
            minX = Math.Min(beacon.X, Math.Min(minX, sensor.X));
            maxX = Math.Max(beacon.X, Math.Max(maxX, sensor.X));
            int delta = ElfHelpers.GetDelta(sensor, beacon);
            maxDelta = Math.Max(delta, maxDelta);
            sensorPositions[count] = new(sensor, delta);
            beaconPositions[count++] = beacon;
        }

        int result = 0;

        // The maximum range along the row is the max delta either side of the min and max x
        for (int x = minX - maxDelta; x <= maxX + maxDelta; ++x)
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