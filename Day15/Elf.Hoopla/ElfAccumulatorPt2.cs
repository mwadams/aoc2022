namespace Elf.Hoopla;

internal readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process(int row)
    {
        return ProcessLines(this.lines, row);
    }

    private static long ProcessLines(string[] lines, int range)
    {
        Span<SignalInfo> sensorPositions = stackalloc SignalInfo[lines.Length];
        Span<Point> beaconPositions = stackalloc Point[lines.Length];
        int count = 0;


        foreach (var line in lines)
        {
            ParseLine(line.AsSpan(), out Point sensor, out Point beacon);
            int delta = Math.Abs(sensor.X - beacon.X) + Math.Abs(sensor.Y - beacon.Y);
            sensorPositions[count] = new(sensor, delta);
            beaconPositions[count++] = beacon;
        }

        foreach(var signalInfo in sensorPositions)
        {
            for(int deltaX = 0; deltaX <= signalInfo.Delta + 1; deltaX++)
            {
                int deltaY = signalInfo.Delta + 1 - deltaX;

                int x = signalInfo.Sensor.X - deltaX;
                int y = signalInfo.Sensor.Y - deltaY;
                if (IsInBounds(x, y, range) && ValidatePoint(x, y, sensorPositions))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X - deltaX;
                y = signalInfo.Sensor.Y + deltaY;
                if (IsInBounds(x, y, range) && ValidatePoint(x, y, sensorPositions))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X + deltaX;
                y = signalInfo.Sensor.Y - deltaY;
                if (IsInBounds(x, y, range) && ValidatePoint(x, y, sensorPositions))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X + deltaX;
                y = signalInfo.Sensor.Y + deltaY;
                if (IsInBounds(x, y, range) && ValidatePoint(x, y, sensorPositions))
                {
                    return CalculateCode(x, y);
                }
            }
        }

        throw new InvalidOperationException();
    }

    private static long CalculateCode(int x, int y)
    {
        return (x * (long)4_000_000) + y;
    }

    private static bool IsInBounds(int x, int y, int range)
    {
        return x >= 0 && y >= 0 && x <= range && y <= range;
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