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
    }

    public void FindLocation()
    {
        this.FindStartAndEnd(out Point start, out Point end);
        Point highPoint = this.FindHighestPointAround(end);
        this.FindPath(start, highPoint);
        this.ShortestPath = this.FindShortestPath(start, highPoint);
    }

    private void FindStartAndEnd(out Point start, out Point end)
    {
        start = new(-1,-1);
        end = new(-1,-1);
        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                char c = this.lines[y][x];
                if (c == 'S')
                {
                    start = new(x, y);
                    if(end.X != -1)
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

    private Point FindHighestPointAround(Point end)
    {
        Point? max = null;

        Span<Point> connections = stackalloc Point[4];
        int written = GetConnections(this.lines, this.width, this.height, end, connections);

        foreach (var point in connections[..written])
        {
            if (max is null || this.lines[point.Y][point.X] > this.lines[max.Value.Y][max.Value.X])
            {
                max = point;
            }
        }

        return max!.Value;
    }

    private int FindShortestPath(Point start, Point end)
    {
        int steps = 1; // 1 extra step because we aren't going to the actual end

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
        visitedNodes.Add(start);
        workingSet.Enqueue(start);
        
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

    private static int GetConnections(string[] lines, int width, int height, Point point, Span<Point> connections)
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
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
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
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
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
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
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
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections[written++] = new(point.X, point.Y + 1);
                }
            }
        }

        return written;
    }
}