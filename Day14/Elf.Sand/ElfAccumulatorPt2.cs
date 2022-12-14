namespace Elf.Sand;

using System;

internal readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public int FindUnits(in Point entry)
    {
        Span<Segment> segments = new Segment[this.lines.Length];
        BuildSegments(lines.AsSpan(), segments, entry, out Point minimum, out Point maximum);
        CalculateDimensionsAndUpdateMinMax(entry, ref minimum, ref maximum, out int width, out int height);

        Span<char> matrixBuffer = new char[width * height];

        Matrix matrix = new(minimum, maximum, matrixBuffer);
        DrawBuffer(segments, ref matrix);
        int result = RunSand(height, entry, ref matrix);
        ////matrix.WriteToFile("pt3.txt");
        return result;
    }

    private static void CalculateDimensionsAndUpdateMinMax(in Point entry, ref Point minimum, ref Point maximum, out int width, out int height)
    {
        width = ((maximum.X - minimum.X) + 1);
        height = ((maximum.Y - minimum.Y) + 1);

        // 2 extra rows for the base
        height += 2;

        int left = entry.X - minimum.X;
        int right = maximum.X - entry.X;
        int deltaLeft = width * 2;
        int deltaRight = deltaLeft;
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

    private static int RunSand(int height, in Point entry, ref Matrix matrix)
    {
        int count = 0;

        int rowIndex = 0;
        for (int y = entry.Y; y < entry.Y + height - 1; ++y)
        {
            for (int x = entry.X - rowIndex; x < entry.X + rowIndex + 1; ++x)
            {
                if (matrix.Get(x, y) == '.' && (rowIndex == 0 || !IsSheltered(x, y, ref matrix)))
                {
                    matrix.Set(x, y, 'O');
                    count++;
                }
            }

            rowIndex++;
        }

        return count;
    }

    private static bool IsSheltered(int x, int y, ref Matrix matrix)
    {
        char p1 = matrix.Get(x, y - 1);
        if (p1 != '#' && p1 != '.')
        {
            return false;
        }

        if (matrix.IsInBounds(x - 1, y - 1))
        {
            char p2 = matrix.Get(x - 1, y - 1);
            if (p2 != '#' && p2 != '.')
            {
                return false;
            }
        }

        if (matrix.IsInBounds(x + 1, y - 1))
        {
            char p3 = matrix.Get(x + 1, y - 1);
            if (p3 != '#' && p3 != '.')
            {
                return false;
            }
        }

        return true;
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