namespace Elf.Hoopla;

using System;

public static class ElfHelpers
{
    public static int GetDelta(in Point p1, in Point p2)
    {
        // I didn't know this kind of delta was called a "Manhattan distances"
        // basically how far do you have to walk down and across in a discrete space
        return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }

    public static int GetDelta(int x1, int y1, int x2, int y2)
    {
        // I didn't know this kind of delta was called a "Manhattan distances"
        // basically how far do you have to walk down and across in a discrete space
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    public static bool IsOutOfRange(int x, int y, ReadOnlySpan<SignalInfo> sensorPositions)
    {
        foreach (var signalInfo in sensorPositions)
        {
            int delta = GetDelta(x, y, signalInfo.Sensor.X, signalInfo.Sensor.Y);
            if (delta <= signalInfo.Delta)
            {
                return false;
            }
        }

        return true;
    }

    public static void ParseLine(ReadOnlySpan<char> line, out Point sensor, out Point beacon)
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
