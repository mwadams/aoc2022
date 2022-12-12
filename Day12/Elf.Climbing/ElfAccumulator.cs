namespace Elf.Climbing;

using System.Collections.Generic;

internal ref struct ElfAccumulator
{
    private readonly string[] lines;
    private readonly int width;
    private readonly int height;
    private readonly Dictionary<Point, Point> parents = new();

    public int ShortestPath { get; private set; }

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
        this.width = lines[0].Length;
        this.height = lines.Length;
        this.ShortestPath = int.MaxValue;
    }

    public void FindLocation()
    {
        this.FindStartAndEnd(out Point start, out Point end);

        this.FindPath(start, end);
        this.ShortestPath = this.FindShortestPath(start, end);
    }

    private void FindStartAndEnd(out Point start, out Point end)
    {
        start = new(-1, -1);
        end = new(-1, -1);
        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                char c = this.lines[y][x];
                if (c == 'S')
                {
                    start = new(x, y);
                    if (end.X != -1)
                    {
                        return;
                    }
                }
                else if (c == 'E')
                {
                    end = new(x, y);
                    if (start.X != -1)
                    {
                        return;
                    }
                }
            }
        }
    }

    private int FindShortestPath(Point start, Point end)
    {
        int steps = 0;

        Point current = end;
        while (current != start)
        {
            steps++;
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