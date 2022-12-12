namespace Elf.Climbing;

using System;
using System.ComponentModel;
using System.Text;

internal ref struct ElfAccumulator
{
    private readonly string[] lines;
    private readonly int width;
    private readonly int height;
    private readonly Node[,] matrix;

    public int ShortestPath { get; private set; }

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
        this.width = lines[0].Length;
        this.height = lines.Length;
        this.matrix = new Node[this.width, this.height];
    }

    public void FindLocation()
    {
        HashSet<Point> unvisitedNodes = this.BuildMap(out Point start, out Point end);
        Point highPoint = this.FindHighestPointAround(end);
        this.FindPath(unvisitedNodes, start, highPoint);
        this.ShortestPath = this.FindShortestPath(start, highPoint);
    }

    private Point FindHighestPointAround(Point end)
    {
        Node? max = null;
        foreach(var point in this.matrix[end.X, end.Y].Connections)
        {
            if (max is null || this.matrix[point.X, point.Y].Height > max.Value.Height)
            {
                max = this.matrix[point.X, point.Y];
            }
        }

        return max!.Value.Location;
    }

    private int FindShortestPath(Point start, Point end)
    {
        int steps = 1; // 1 extra step because we aren't going to the actual end

        Point current = end;
        while (current != start)
        {
            steps++;
            current = this.matrix[current.X, current.Y].Parent;
        }

        return steps;
    }

    private void FindPath(HashSet<Point> unvisitedNodes, Point start, Point end)
    {
        Queue<Point> workingSet = new();
        unvisitedNodes.Remove(start);
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
                foreach (Point connection in this.matrix[current.X, current.Y].Connections)
                {
                    if (unvisitedNodes.Contains(connection))
                    {
                        unvisitedNodes.Remove(connection);
                        this.matrix[connection.X, connection.Y].Parent = current;
                        workingSet.Enqueue(connection);
                    }
                }
            }
        }
    }

    private HashSet<Point> BuildMap(out Point start, out Point end)
    {
        start = new Point(0, 0);
        end = new Point(0, 0);
        HashSet<Point> unvisitedNodes = new();
        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                char c = lines[y][x];

                if (c == 'S')
                {
                    start = new Point(x, y);
                }
                else if (c == 'E')
                {
                    end = new Point(x, y);
                }

                var node = new Node(new(x, y), c, BuildConnections(c, x, y), new(-1,-1));
                matrix[x, y] = node;
                unvisitedNodes.Add(new(x, y));
            }
        }

        return unvisitedNodes;
    }

    private List<Point> BuildConnections(char c, int x, int y)
    {
        List<Point> connections = new();
        bool isStartOrEnd = c == 'S' || c == 'E';

        if (x > 0)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(x - 1, y));
            }
            else
            {
                char c2 = this.lines[y][x - 1];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(x - 1, y));
                }
            }
        }

        if (y > 0)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(x, y - 1));
            }
            else
            {
                char c2 = this.lines[y - 1][x];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(x, y - 1));
                }
            }
        }

        if (x < width - 1)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(x + 1, y));
            }
            else
            {
                char c2 = this.lines[y][x + 1];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(x + 1, y));
                }
            }
        }

        if (y < height - 1)
        {
            if (isStartOrEnd)
            {
                connections.Add(new(x, y + 1));
            }
            else
            {
                char c2 = this.lines[y + 1][x];
                if (c2 != 'E' && c2 != 'S' && c2 - c <= 1)
                {
                    connections.Add(new(x, y + 1));
                }
            }
        }

        return connections;
    }
}