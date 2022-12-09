namespace Elf.Shenanigans;

using System;

internal ref struct ElfAccumulator
{
    (int X, int Y) headPosition;
    (int X, int Y)[] tailPosition;

    HashSet<(int, int)> tailPositions;
    private readonly int maxDistance;

    public ElfAccumulator(int maxDistance, int knots)
    {
        tailPositions = new();
        headPosition = (0, 0);
        tailPosition = new(int X, int Y)[knots];
        tailPositions.Add((0, 0));
        this.maxDistance = maxDistance;
    }

    public int Visited => tailPositions.Count;

    public void ProcessLine(string line)
    {
        int distance = int.Parse(line.AsSpan()[2..]);
        switch (line[0])
        {
            case 'U':
                HeadUp(distance);
                break;
            case 'D':
                HeadDown(distance);
                break;
            case 'L':
                HeadLeft(distance);
                break;
            case 'R':
                HeadRight(distance);
                    break;
            default:
                throw new InvalidOperationException("Unexpected direction.");
        };
    }

    private void HeadRight(int distance)
    {
        for(int i = 0; i < distance; ++i)
        {
            headPosition.X++;
            UpdateTails();
        }
    }

    private void HeadLeft(int distance)
    {
        for (int i = 0; i < distance; ++i)
        {
            headPosition.X--;
            UpdateTails();
        }
    }

    private void HeadDown(int distance)
    {
        for (int i = 0; i < distance; ++i)
        {
            headPosition.Y--;
            UpdateTails();
        }
    }

    public void HeadUp(int distance)
    {
        for (int i = 0; i < distance; ++i)
        {
            headPosition.Y++;
            UpdateTails();
        }
    }

    private void UpdateTails()
    {
        for(int i = 0; i < tailPosition.Length; ++i)
        {
            UpdateTail(i);
        }
    }

    private void UpdateTail(int index)
    {
        (int X, int Y) compareTo = index > 0 ? tailPosition[index - 1] : headPosition;

        if (compareTo.X > tailPosition[index].X + this.maxDistance)
        {
            tailPosition[index].X++;
            UpdateYAfterX(index, compareTo);
        }
        else if (compareTo.X < tailPosition[index].X - this.maxDistance)
        {
            tailPosition[index].X--;
            UpdateYAfterX(index, compareTo);
        }
        else if (compareTo.Y > tailPosition[index].Y + this.maxDistance)
        {
            tailPosition[index].Y++;
            UpdateXAfterY(index, compareTo);
        }
        else if (compareTo.Y < tailPosition[index].Y - this.maxDistance)
        {
            tailPosition[index].Y--;
            UpdateXAfterY(index, compareTo);
        }

        if (index == tailPosition.Length - 1)
        {
            tailPositions.Add(tailPosition[index]);
        }
    }

    private void UpdateYAfterX(int index, (int X, int Y) compareTo)
    {
        if (compareTo.Y > tailPosition[index].Y)
        {
            tailPosition[index].Y++;
        }
        else if (compareTo.Y < tailPosition[index].Y)
        {
            tailPosition[index].Y--;
        }
    }

    private void UpdateXAfterY(int index, (int X, int Y) compareTo)
    {
        if (compareTo.X > tailPosition[index].X)
        {
            tailPosition[index].X++;
        }
        else if (compareTo.X < tailPosition[index].X)
        {
            tailPosition[index].X--;
        }
    }
}