namespace Elf.Climbing;

using System.Collections.Generic;

internal ref struct ElfAccumulatorScenic
{
    private readonly string[] lines;
    private readonly int width;
    private readonly int height;
    private readonly Dictionary<Point, Point> parents = new();

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

        HashSet<Point> visitedBs = new();

        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                if (this.lines[y][x] == 'S' || this.lines[y][x] == 'a')
                {
                    Point start = new(x, y);
                    if (HasSurroundingB(start, visitedBs))
                    {
                        this.FindPath(start, end);
                        this.ShortestPath = this.FindShortestPath(this.ShortestPath, start, end);
                    }
                }
            }
        }
    }

    private bool HasSurroundingB(Point point, HashSet<Point> visitedBs)
    {
        if (point.X > 0)
        {
            Point p = new(point.X - 1, point.Y);
            char c2 = lines[p.Y][p.X];
            if (c2 == 'b')
            {
                if (!visitedBs.Contains(p))
                {
                    visitedBs.Add(p);
                    return true;
                }
            }
        }

        if (point.Y > 0)
        {
            Point p = new(point.X, point.Y - 1);
            char c2 = lines[p.Y][p.X];
            if (c2 == 'b')
            {
                if (!visitedBs.Contains(p))
                {
                    visitedBs.Add(p);
                    return true;
                }
            }
        }

        if (point.X < this.width - 1)
        {
            Point p = new(point.X + 1, point.Y);
            char c2 = lines[p.Y][p.X];
            if (c2 == 'b')
            {
                if (!visitedBs.Contains(p))
                {
                    visitedBs.Add(p);
                    return true;
                }
            }
        }

        if (point.Y < this.height - 1)
        {
            Point p = new(point.X, point.Y + 1);
            char c2 = lines[p.Y][p.X];
            if (c2 == 'b')
            {
                if (!visitedBs.Contains(p))
                {
                    visitedBs.Add(p);
                    return true;
                }
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
                    return;
                }
            }
        }
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
                int written = GetConnections(this.lines, this.width, this.height, current, connections);
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

    private static int GetConnections(in string[] lines, int width, int height, Point point, Span<Point> connections)
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
                    c2 = 'z';
                }

                if (c2 != 'a' && c2 != 'S' && c2 - c <= 1)
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
                    c2 = 'z';
                }

                if (c2 != 'a' && c2 != 'S' && c2 - c <= 1)
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
                    c2 = 'z';
                }

                if (c2 != 'a' && c2 != 'S' && c2 - c <= 1)
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
                    c2 = 'z';
                }

                if (c2 != 'a' && c2 != 'S' && c2 - c <= 1)
                {
                    connections[written++] = new(point.X, point.Y + 1);
                }
            }
        }

        return written;
    }
}