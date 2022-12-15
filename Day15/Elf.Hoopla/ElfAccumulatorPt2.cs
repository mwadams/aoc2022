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
            // We are going to walk around all the points exactly one unit out of range of this sensor
            // That might be:
            //    zero steps in the x direction
            //    and the signal's full delta (plus 1 to to get out of range) in the y (either up or down)
            // all the way through to
            //   the signal's full delta (plus 1 to get out of range) in the x direction (either left or right)
            //   and zero steps in the y direction.
            for(int deltaX = 0; deltaX <= signalInfo.Delta + 1; deltaX++)
            {
                // This does the triangulation of the walk
                // (if we went x across, there's this many "up/down" to go (plus the 1 to get out of range)
                int deltaY = signalInfo.Delta + 1 - deltaX;

                // Walk out in each of the four directions in turn
                int x = signalInfo.Sensor.X - deltaX;
                int y = signalInfo.Sensor.Y - deltaY;
                if (PointIsInBoundsAndOutOfRangeOfAllSensors(range, sensorPositions, x, y))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X - deltaX;
                y = signalInfo.Sensor.Y + deltaY;
                if (PointIsInBoundsAndOutOfRangeOfAllSensors(range, sensorPositions, x, y))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X + deltaX;
                y = signalInfo.Sensor.Y - deltaY;
                if (PointIsInBoundsAndOutOfRangeOfAllSensors(range, sensorPositions, x, y))
                {
                    return CalculateCode(x, y);
                }

                x = signalInfo.Sensor.X + deltaX;
                y = signalInfo.Sensor.Y + deltaY;
                if (PointIsInBoundsAndOutOfRangeOfAllSensors(range, sensorPositions, x, y))
                {
                    return CalculateCode(x, y);
                }
            }
        }

        throw new InvalidOperationException();
    }

    private static bool PointIsInBoundsAndOutOfRangeOfAllSensors(int range, Span<SignalInfo> sensorPositions, int x, int y)
    {
        return IsInBounds(x, y, range) && ElfHelpers.IsOutOfRange(x, y, sensorPositions);
    }

    private static bool IsInBounds(int x, int y, int range)
    {
        return x >= 0 && y >= 0 && x <= range && y <= range;
    }

    private static long CalculateCode(int x, int y)
    {
        return (x * (long)4_000_000) + y;
    }
}