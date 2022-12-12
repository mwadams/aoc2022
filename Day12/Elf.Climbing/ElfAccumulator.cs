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
        start = default;
        end = default;
        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                if (this.lines[y][x] == 'S')
                {
                    start = new(x, y);
                }
                else if (this.lines[y][x] == 'E')
                {
                    end = new(x, y);
                }
            }
        }
    }

    private Point FindHighestPointAround(Point end)
    {
        Point? max = null;

        foreach (var point in GetConnections(end))
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
        while (workingSet.Count > 0)
        {
            Point current = workingSet.Dequeue();
            if (current == end)
            {
                break;
            }
            else
            {
                foreach (Point connection in GetConnections(current))
                {
                    if (!visitedNodes.Contains(connection))
                    {
                        visitedNodes.Add(connection);
                        this.parents[new(connection.X, connection.Y)] = current;
                        workingSet.Enqueue(connection);
                    }
                }
            }
        }
    }

    private List<Point> GetConnections(Point point)
    {
        List<Point> connections = new();
        char c = this.lines[point.Y][point.X];
        bool isStartOrEnd = c == 'S' || c == 'E';

        if (point.X > 0)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(point.X - 1, point.Y));
            }
            else
            {
                char c2 = this.lines[point.Y][point.X - 1];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(point.X - 1, point.Y));
                }
            }
        }

        if (point.Y > 0)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(point.X, point.Y - 1));
            }
            else
            {
                char c2 = this.lines[point.Y - 1][point.X];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(point.X, point.Y - 1));
                }
            }
        }

        if (point.X < width - 1)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(point.X + 1, point.Y));
            }
            else
            {
                char c2 = this.lines[point.Y][point.X + 1];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(point.X + 1, point.Y));
                }
            }
        }

        if (point.Y < height - 1)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(point.X, point.Y + 1));
            }
            else
            {
                char c2 = this.lines[point.Y + 1][point.X];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(point.X, point.Y + 1));
                }
            }
        }

        return connections;
    }
}