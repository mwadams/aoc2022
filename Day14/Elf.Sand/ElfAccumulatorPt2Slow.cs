namespace Elf.Sand;

using System;
using System.Collections.Generic;

internal readonly ref struct ElfAccumulatorPt2Slow
{
    private readonly string[] lines;

    public ElfAccumulatorPt2Slow(string[] lines)
    {
        this.lines = lines;
    }

    public int FindUnits(Point entry)
    {
        Span<Segment> segments = new Segment[this.lines.Length];
        BuildSegments(lines.AsSpan(), segments, entry, out Point minimum, out Point maximum);
        CalculateDimensionsAndUpdateMinMax(entry, ref minimum, ref maximum, out int width, out int height);

        Span<char> matrixBuffer = new char[width * height];

        Matrix matrix = new(minimum, maximum, matrixBuffer);
        DrawBuffer(segments, ref matrix);
        int result = RunSand(entry, ref matrix);
        ////matrix.WriteToFile("pt2.txt");
        return result;
    }

    private static void CalculateDimensionsAndUpdateMinMax(Point entry, ref Point minimum, ref Point maximum, out int width, out int height)
    {
        width = ((maximum.X - minimum.X) + 1);
        height = ((maximum.Y - minimum.Y) + 1);

        // 2 extra rows for the base
        height += 2;

        int left = entry.X - minimum.X;
        int right = maximum.X - entry.X;
        int deltaLeft = width * 2;
        int deltaRight = width * 2;
        if (left < right)
        {
            deltaLeft += right - left;
        }
        else if (right < left)
        {
            deltaRight += left - right;
        }

        width = width + deltaLeft + deltaRight;
        if (width < height)
        {
            int delta = ((height - width) + 1) / 2;
            deltaLeft += delta;
            deltaRight += delta;
            width += delta * 2;
        }

        minimum = new(minimum.X - deltaLeft, minimum.Y);
        maximum = new(maximum.X + deltaRight, maximum.Y + 2);
    }

    private static int RunSand(in Point entry, ref Matrix matrix)
    {
        int count = 0;
        Point current;
        do
        {
            DropSand(entry, ref matrix, out current);
            count++;
        }
        while (current != entry);

        return count;
    }

    enum Result { Moving, Stopped, FlowedAway };

    private static void DropSand(in Point entry, ref Matrix matrix, out Point current)
    {
        Result result = Result.Moving;
        Point c = entry;
        while (result == Result.Moving)
        {
            result = TryMove(ref c, ref matrix);
        }

        current = c;
    }

    private static Result TryMove(ref Point current, ref Matrix matrix)
    {
        Point candidate = new(current.X, current.Y + 1);

        if (!matrix.IsInBounds(candidate))
        {
            return Result.FlowedAway;
        }

        if (TestCandidate(ref matrix, candidate))
        {
            current = candidate;
            return Result.Moving;
        }

        candidate = new(current.X - 1, current.Y + 1);
        if (!matrix.IsInBounds(candidate))
        {
            return Result.FlowedAway;
        }

        if (TestCandidate(ref matrix, candidate))
        {
            current = candidate;
            return Result.Moving;
        }

        candidate = new(current.X + 1, current.Y + 1);
        if (!matrix.IsInBounds(candidate))
        {
            return Result.FlowedAway;
        }

        if (TestCandidate(ref matrix, candidate))
        {
            current = candidate;
            return Result.Moving;
        }

        matrix.Set(current, 'O');

        return Result.Stopped;

        static bool TestCandidate(ref Matrix matrix, in Point candidate)
        {
            return matrix.Get(candidate) == '.' && !matrix.IsOnFloor(candidate.Y);
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

    private static void BuildSegments(ReadOnlySpan<string> lines, Span<Segment> segments, Point entry, out Point minimum, out Point maximum)
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

        int minX = minimum.X, minY = minimum.Y, maxX = maximum.X, maxY = maximum.Y;

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
            minY = Math.Min(minY, start.Y);
            maxX = Math.Max(maxX, start.X);
            maxY = Math.Max(maxY, start.Y);
            segments[segmentCount++] = start;
            i += consumed;
        }

        minimum = new Point(minX, minY);
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