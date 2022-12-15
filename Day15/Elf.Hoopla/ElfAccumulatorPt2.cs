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
            ElfHelpers.ParseLine(line.AsSpan(), out Point sensor, out Point beacon);
            int delta = ElfHelpers.GetDelta(sensor, beacon);
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
                if (IsInBounds(x, y, range) && ElfHelpers.IsOutOfRange(x, y, sensorPositions))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X - deltaX;
                y = signalInfo.Sensor.Y + deltaY;
                if (IsInBounds(x, y, range) && ElfHelpers.IsOutOfRange(x, y, sensorPositions))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X + deltaX;
                y = signalInfo.Sensor.Y - deltaY;
                if (IsInBounds(x, y, range) && ElfHelpers.IsOutOfRange(x, y, sensorPositions))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X + deltaX;
                y = signalInfo.Sensor.Y + deltaY;
                if (IsInBounds(x, y, range) && ElfHelpers.IsOutOfRange(x, y, sensorPositions))
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
}