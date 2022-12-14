namespace Elf.Sand;

using System;
using System.Collections.Generic;

internal readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public int FindUnits(in Point entry)
    {
        Span<Segment> segments = new Segment[this.lines.Length];
        BuildSegments(lines.AsSpan(), segments, entry, out Point minimum, out Point maximum);
        Span<char> matrixBuffer = new char[((maximum.X - minimum.X) + 1) * ((maximum.Y - minimum.Y) + 1)];
        Matrix matrix = new(minimum, maximum, matrixBuffer);
        DrawBuffer(segments, ref matrix);
        int result = RunSand(entry, ref matrix);
        matrix.WriteToFile("pt1.txt", entry);
        return result;
    }

    private static int RunSand(in Point entry, ref Matrix matrix)
    {
        int count = 0;
        while (true)
        {
            if (DropSand(entry, ref matrix))
            {
                count++;
            }
            else
            {
                return count;
            }
        }
    }

    enum Result { Moving, Stopped, FlowedAway };

    private static bool DropSand(in Point entry, ref Matrix matrix)
    {
        Result result = Result.Moving;
        Point current = entry;
        while (result == Result.Moving)
        {
            result = TryMove(ref current, ref matrix);
        }

        return result == Result.Stopped;
    }

    private static Result TryMove(ref Point current, ref Matrix matrix)
    {
        int x = current.X;
        int y = current.Y + 1;

        if (!matrix.IsInBounds(x, y))
        {
            return Result.FlowedAway;
        }

        if (TestCandidate(ref matrix, x, y))
        {
            current = new(x, y);
            return Result.Moving;
        }

        x = x - 1;
        if (!matrix.IsInBoundsH(x))
        {
            return Result.FlowedAway;
        }

        if (TestCandidate(ref matrix, x, y))
        {
            current = new(x, y);
            return Result.Moving;
        }

        x = x + 2;
        if (!matrix.IsInBoundsH(x))
        {
            return Result.FlowedAway;
        }

        if (TestCandidate(ref matrix, x, y))
        {
            current = new(x, y);
            return Result.Moving;
        }

        matrix.Set(current, 'O');

        return Result.Stopped;

        static bool TestCandidate(ref Matrix matrix, int x, int y)
        {
            return matrix.Get(x, y) == '.';
        }
    }

    private static void DrawBuffer(Span<Segment> segments, ref Matrix matrix)
    {
        foreach (var segment in segments)
        {
            for (int i = 1; i < segment.Points.Length; ++i)
            {
                DrawSegment(segment.Points[i - 1], segment.Points[i], ref matrix);
            }
        }
    }

    private static void DrawSegment(in Point start, in Point end, ref Matrix matrix)
    {
        if (start.X == end.X)
        {
            for (int i = Math.Min(start.Y, end.Y); i <= Math.Max(start.Y, end.Y); ++i)
            {
                matrix.Set(new(start.X, i), '#');
            }
        }
        else
        {
            for (int i = Math.Min(start.X, end.X); i <= Math.Max(start.X, end.X); ++i)
            {
                matrix.Set(new(i, start.Y), '#');
            }
        }
    }

    private static void BuildSegments(ReadOnlySpan<string> lines, Span<Segment> segments, in Point entry, out Point minimum, out Point maximum)
    {
        Point min = entry;
        Point max = entry;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            segments[i] = ParseLine(line.AsSpan(), ref min, ref max);
        }

        minimum = min;
        maximum = max;
    }

    private static Segment ParseLine(ReadOnlySpan<char> line, ref Point minimum, ref Point maximum)
    {
        Span<Point> segments = stackalloc Point[32];

        int minX = minimum.X, maxX = maximum.X, maxY = maximum.Y;

        int segmentCount = 0;
        for (int i = 0; i < line.Length; ++i)
        {
            if (line[i] == '-')
            {
                i += 2; // skip a connector
                continue;
            }
            int consumed = ParsePoint(line[i..], out Point start);
            minX = Math.Min(minX, start.X);
            maxX = Math.Max(maxX, start.X);
            maxY = Math.Max(maxY, start.Y);
            segments[segmentCount++] = start;
            i += consumed;
        }

        minimum = new Point(minX, 0);
        maximum = new Point(maxX, maxY);
        return new Segment(segments[..segmentCount].ToArray());
    }

    private static int ParsePoint(ReadOnlySpan<char> line, out Point start)
    {
        int commaIndex = line.IndexOf(',');
        int spaceIndex = line[(commaIndex + 2)..].IndexOf(' ');
        if (spaceIndex != -1)
        {
            int consumed = spaceIndex + commaIndex + 2;
            start = new Point(int.Parse(line[..commaIndex]), int.Parse(line[(commaIndex + 1)..consumed]));
            return consumed;
        }
        else
        {
            start = new Point(int.Parse(line[..commaIndex]), int.Parse(line[(commaIndex + 1)..]));
            return line.Length;
        }
    }
}