namespace Elf.Climbing;

using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

internal ref struct ElfAccumulatorScenic
{
    private readonly string[] lines;
    private readonly int width;
    private readonly int height;
    private readonly Dictionary<Point, Point> parents = new();
    private char endHeight;

    public int ShortestPath { get; private set; }

    public ElfAccumulatorScenic(string[] lines)
    {
        this.lines = lines;
        this.width = lines[0].Length;
        this.height = lines.Length;
        this.ShortestPath = int.MaxValue;
    }

    public void FindLocation()
    {
        this.FindEnd(out Point end);

        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                if (this.lines[y][x] == 'S' || this.lines[y][x] == 'a')
                {
                    Point start = new(x, y);
                    if (HasSurroundingB(start))
                    {
                        this.FindPath(start, end);
                        this.ShortestPath = this.FindShortestPath(this.ShortestPath, start, end);
                    }
                }
            }
        }
    }

    private bool HasSurroundingB(Point point)
    {
        if (point.X > 0)
        {
            char c2 = lines[point.Y][point.X - 1];
            if (c2 == 'b')
            {
                return true;
            }
        }

        if (point.Y > 0)
        {
            char c2 = lines[point.Y - 1][point.X];
            if (c2 == 'b')
            {
                return true;
            }
        }

        if (point.X < this.width - 1)
        {
            char c2 = lines[point.Y][point.X + 1];
            if (c2 == 'b')
            {
                return true;
            }
        }

        if (point.Y < this.height - 1)
        {
            char c2 = lines[point.Y + 1][point.X];
            if (c2 == 'b')
            {
                return true;
            }
        }

        return false;
    }

    private void FindEnd(out Point end)
    {
        end = new(-1, -1);
        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                char c = this.lines[y][x];
                if (c == 'E')
                {
                    end = new(x, y);
                    this.endHeight = FindHighestPointAround(end);
                    return;
                }
            }
        }
    }

    private char FindHighestPointAround(Point end)
    {
        Point? max = null;

        Span<Point> connections = stackalloc Point[4];
        int written = GetConnections(this.lines, this.width, this.height, end, connections, 'E');

        foreach (var point in connections[..written])
        {
            if (max is null || this.lines[point.Y][point.X] > this.lines[max.Value.Y][max.Value.X])
            {
                max = point;
            }
        }

        return this.lines[max!.Value.Y][max!.Value.X];
    }

    private int FindShortestPath(int currentShortest, Point start, Point end)
    {
        int steps = 0;

        Point current = end;
        while (current != start)
        {
            steps++;
            if (steps > currentShortest)
            {
                return currentShortest;
            }

            current = this.parents[current];
        }

        return steps;
    }

    private void FindPath(Point start, Point end)
    {
        HashSet<Point> visitedNodes = new();
        Queue<Point> workingSet = new();

        workingSet.Enqueue(start);
        visitedNodes.Add(start);

        Span<Point> connections = stackalloc Point[4];

        while (workingSet.Count > 0)
        {
            Point current = workingSet.Dequeue();
            if (current == end)
            {
                break;
            }
            else
            {
                int written = GetConnections(this.lines, this.width, this.height, current, connections, this.endHeight);
                foreach (Point connection in connections[..written])
                {
                    if (!visitedNodes.Contains(connection))
                    {
                        visitedNodes.Add(connection);
                        this.parents[connection] = current;
                        workingSet.Enqueue(connection);
                    }
                }
            }
        }
    }

    private static int GetConnections(string[] lines, int width, int height, Point point, Span<Point> connections, char endHeight)
    {
        int written = 0;
        char c = lines[point.Y][point.X];
        bool isStartOrEnd = c == 'S' || c == 'E';

        if (point.X > 0)
        {
            if (isStartOrEnd)
            {
                connections[written++] = new(point.X - 1, point.Y);
            }
            else
            {
                char c2 = lines[point.Y][point.X - 1];
                if (c2 == 'E')
                {
                    c2 = endHeight;
                }
                if (c2 != 'S' && c2 - c <= 1)
                {
                    connections[written++] = new(point.X - 1, point.Y);
                }
            }
        }

        if (point.Y > 0)
        {
            if (isStartOrEnd)
            {
                connections[written++] = new(point.X, point.Y - 1);
            }
            else
            {
                char c2 = lines[point.Y - 1][point.X];
                if (c2 == 'E')
                {
                    c2 = endHeight;
                }
                if (c2 != 'S' && c2 - c <= 1)
                {
                    connections[written++] = new(point.X, point.Y - 1);
                }
            }
        }

        if (point.X < width - 1)
        {
            if (isStartOrEnd)
            {
                connections[written++] = new(point.X + 1, point.Y);
            }
            else
            {
                char c2 = lines[point.Y][point.X + 1];
                if (c2 == 'E')
                {
                    c2 = endHeight;
                }
                if (c2 != 'S' && c2 - c <= 1)
                {
                    connections[written++] = new(point.X + 1, point.Y);
                }
            }
        }

        if (point.Y < height - 1)
        {
            if (isStartOrEnd)
            {
                connections[written++] = new(point.X, point.Y + 1);
            }
            else
            {
                char c2 = lines[point.Y + 1][point.X];
                if (c2 == 'E')
                {
                    c2 = endHeight;
                }
                if (c2 != 'S' && c2 - c <= 1)
                {
                    connections[written++] = new(point.X, point.Y + 1);
                }
            }
        }

        return written;
    }
}