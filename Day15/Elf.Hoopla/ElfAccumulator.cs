namespace Elf.Hoopla;

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
            ParseLine(line.AsSpan(), out Point sensor, out Point beacon);
            minX = Math.Min(beacon.X, Math.Min(minX, sensor.X));
            maxX = Math.Max(beacon.X, Math.Max(maxX, sensor.X));
            int delta = Math.Abs(sensor.X - beacon.X) + Math.Abs(sensor.Y - beacon.Y);
            maxDelta = Math.Max(delta, maxDelta);
            sensorPositions[count] = new(sensor, delta);
            beaconPositions[count++] = beacon;
        }

        int result = 0;

        for (int x = minX - maxDelta; x <= maxX + maxDelta; ++x)
        {
            if (!ValidatePoint(x, row, sensorPositions) && !beaconPositions.Contains(new(x, row)))
            {
                result++;
            }
        }

        return result;
    }

    private static bool ValidatePoint(int x, int y, ReadOnlySpan<SignalInfo> sensorPositions)
    {
        foreach (var signalInfo in sensorPositions)
        {
            int delta = Math.Abs(x - signalInfo.Sensor.X) + Math.Abs(y - signalInfo.Sensor.Y);
            if (delta <= signalInfo.Delta)
            {
                return false;
            }
        }

        return true;
    }

    private static void ParseLine(ReadOnlySpan<char> line, out Point sensor, out Point beacon)
    {
        int x1Start = line.IndexOf('x') + 2;
        int x1End = line[x1Start..].IndexOf(',') + x1Start;

        int y1Start = line[x1End..].IndexOf('y') + 2 + x1End;
        int y1End = line[y1Start..].IndexOf(':') + y1Start;

        int x2Start = line[y1End..].IndexOf('x') + 2 + y1End;
        int x2End = line[x2Start..].IndexOf(',') + x2Start;

        int y2Start = line[x2End..].IndexOf('y') + 2 + x2End;

        ReadOnlySpan<char> x1Line = line[x1Start..x1End];
        int x1 = int.Parse(x1Line);
        ReadOnlySpan<char> y1Line = line[y1Start..y1End];
        int y1 = int.Parse(y1Line);
        ReadOnlySpan<char> x2Line = line[x2Start..x2End];
        int x2 = int.Parse(x2Line);
        ReadOnlySpan<char> y2Line = line[y2Start..];
        int y2 = int.Parse(y2Line);
        sensor = new(x1, y1);
        beacon = new(x2, y2);
    }
}