namespace Elf.Shenanigans;

using System;

internal ref struct ElfAccumulator
{
    (int X, int Y) headPosition;
    (int X, int Y) tailPosition;

    HashSet<(int, int)> tailPositions;
    private readonly int maxDistance;

    public ElfAccumulator(int maxDistance)
    {
        tailPositions = new();
        headPosition = (0, 0);
        tailPosition = (0, 0);
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
            headPosition.X++;;
            UpdateTail();
        }
    }

    private void HeadLeft(int distance)
    {
        for (int i = 0; i < distance; ++i)
        {
            headPosition.X--;
            UpdateTail();
        }
    }

    private void HeadDown(int distance)
    {
        for (int i = 0; i < distance; ++i)
        {
            headPosition.Y--;
            UpdateTail();
        }
    }

    public void HeadUp(int distance)
    {
        for (int i = 0; i < distance; ++i)
        {
            headPosition.Y++;
            UpdateTail();
        }
    }

    private void UpdateTail()
    {
        if (headPosition.X > tailPosition.X + this.maxDistance)
        {
            tailPosition.X++;
            if (headPosition.Y > tailPosition.Y)
            {
                tailPosition.Y++;
            }
            else if (headPosition.Y < tailPosition.Y)
            {
                tailPosition.Y--;
            }
        }
        else if (headPosition.X < tailPosition.X - this.maxDistance)
        {
            tailPosition.X--;
            if (headPosition.Y > tailPosition.Y)
            {
                tailPosition.Y++;
            }
            else if (headPosition.Y < tailPosition.Y)
            {
                tailPosition.Y--;
            }
        }
        else if (headPosition.Y > tailPosition.Y + this.maxDistance)
        {
            tailPosition.Y++;
            if (headPosition.X > tailPosition.X)
            {
                tailPosition.X++;
            }
            else if (headPosition.X < tailPosition.X)
            {
                tailPosition.X--;
            }
        }
        else if (headPosition.Y < tailPosition.Y - this.maxDistance)
        {
            tailPosition.Y--;
            if (headPosition.X > tailPosition.X)
            {
                tailPosition.X++;
            }
            else if (headPosition.X < tailPosition.X)
            {
                tailPosition.X--;
            }
        }

        this.tailPositions.Add(this.tailPosition);
    }
}