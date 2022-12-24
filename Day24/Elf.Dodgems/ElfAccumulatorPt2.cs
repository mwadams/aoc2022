namespace Elf.Dodgems;

public readonly ref struct ElfAccumulatorPt2
{
    private static readonly Delta[] DirectionDeltas = new Delta[]
    {
        new(1,0),
        new(0,1),
        new(-1,0),
        new(0,-1)
    };

    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public int Process()
    {
        int height = lines.Length - 2;
        int width = lines[0].Length - 2;

        Dictionary<Point, BlizzardCell> blizzardMap1 = new Dictionary<Point, BlizzardCell>(width * height);
        Dictionary<Point, BlizzardCell> blizzardMap2 = new Dictionary<Point, BlizzardCell>(width * height);
        HashSet<Point> locationBuffer1 = new(width * height);
        HashSet<Point> locationBuffer2 = new(width * height);
        HashSet<Point> currentBuffer = locationBuffer1;
        HashSet<Point> targetBuffer = locationBuffer2;
        Dictionary<Point, BlizzardCell> currentBlizzards = blizzardMap1;
        Dictionary<Point, BlizzardCell> targetBlizzards = blizzardMap2;

        BuildBlizzards(lines, currentBlizzards);

        currentBuffer.Add(new Point(0, -1));

        // This is the point over the exit
        Point goal = new(width - 1, height - 1);
        int time = 0;

        State state = State.HeadingToEnd;

        while (true)
        {
            time++;
            targetBlizzards.Clear();
            targetBuffer.Clear();
            MoveBlizzards(currentBlizzards, targetBlizzards, width, height);

            if (state == State.TurningAround)
            {
                state = State.HeadingToStart;
                targetBuffer.Clear();
                targetBuffer.Add(new(width - 1, height));
            }
            else if (state == State.TurningAroundAgain)
            {
                state = State.HeadingToEndAgain;
                targetBuffer.Clear();
                targetBuffer.Add(new(0, -1));
            }
            else
            {
                foreach (Point p in currentBuffer)
                {
                    if (AddPossibilities(p, targetBlizzards, width, height, targetBuffer, goal))
                    {
                        switch (state)
                        {
                            case State.HeadingToEnd:
                                state = State.TurningAround;
                                goal = new(0, 0);
                                break;
                            case State.HeadingToStart:
                                state = State.TurningAroundAgain;
                                goal = new(width - 1, height - 1);
                                break;
                            case State.HeadingToEndAgain:
                                return time + 1;
                        }
                        // No need to do any more points
                        break;
                    }
                }
            }

            (targetBlizzards, currentBlizzards) = (currentBlizzards, targetBlizzards);
            (targetBuffer, currentBuffer) = (currentBuffer, targetBuffer);
        }
    }

    private static void DrawBlizzards(Dictionary<Point, BlizzardCell> targetBlizzards, HashSet<Point> targetBuffer, int width, int height, int time)
    {
        Console.WriteLine();
        Console.WriteLine($"T: {time}");
        Console.Write("#.");
        for (int x = 0; x < width; x++)
        {
            Console.Write('#');
        }

        for (int y = 0; y < height; y++)
        {
            Console.WriteLine();
            Console.Write('#');
            for (int x = 0; x < width; x++)
            {
                BlizzardCell cell = GetBlizzard(x, y, targetBlizzards);
                if (cell.Blizzards == Directions.Right)
                {
                    Console.Write('>');
                }
                else if (cell.Blizzards == Directions.Down)
                {
                    Console.Write('v');
                }
                else if (cell.Blizzards == Directions.Left)
                {
                    Console.Write('<');
                }
                else if (cell.Blizzards == Directions.Up)
                {
                    Console.Write('^');
                }
                else if (cell.Blizzards != 0)
                {
                    Console.Write(CountBits(cell));
                }
                else
                {
                    if (targetBuffer.Contains(new(x, y)))
                    {
                        Console.Write('E');
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
            }
            Console.Write('#');
        }

        Console.WriteLine();
        for (int x = 0; x < width; x++)
        {
            Console.Write('#');
        }

        Console.Write(".#");

        Console.WriteLine();
        Console.ReadLine();
    }

    private static int CountBits(in BlizzardCell blizzard)
    {
        int count = 0;
        if (blizzard.HasRight)
        {
            count++;
        }

        if (blizzard.HasDown)
        {
            count++;
        }

        if (blizzard.HasLeft)
        {
            count++;
        }

        if (blizzard.HasUp)
        {
            count++;
        }

        return count;
    }

    private static void MoveBlizzards(Dictionary<Point, BlizzardCell> currentBlizzards, Dictionary<Point, BlizzardCell> targetBlizzards, int width, int height)
    {
        foreach ((Point p, BlizzardCell blizzard) in currentBlizzards)
        {
            if (blizzard.HasRight)
            {
                SetBlizzard(MoveBlizzardRight(p, width, height), targetBlizzards, Directions.Right);
            }

            if (blizzard.HasDown)
            {
                SetBlizzard(MoveBlizzardDown(p, width, height), targetBlizzards, Directions.Down);
            }

            if (blizzard.HasLeft)
            {
                SetBlizzard(MoveBlizzardLeft(p, width, height), targetBlizzards, Directions.Left);
            }

            if (blizzard.HasUp)
            {
                SetBlizzard(MoveBlizzardUp(p, width, height), targetBlizzards, Directions.Up);
            }
        }
    }

    private static void SetBlizzard(Point point, Dictionary<Point, BlizzardCell> targetBlizzards, Directions direction)
    {
        BlizzardCell blizzard = GetBlizzard(point.X, point.Y, targetBlizzards);
        targetBlizzards[point] = new(blizzard.Blizzards | direction);
    }

    private static Point MoveBlizzardRight(in Point p, int width, int height)
    {
        return MoveBlizzard(p, DirectionDeltas[0], width, height);
    }

    private static Point MoveBlizzardDown(in Point p, int width, int height)
    {
        return MoveBlizzard(p, DirectionDeltas[1], width, height);
    }

    private static Point MoveBlizzardLeft(in Point p, int width, int height)
    {
        return MoveBlizzard(p, DirectionDeltas[2], width, height);
    }
    private static Point MoveBlizzardUp(in Point p, int width, int height)
    {
        return MoveBlizzard(p, DirectionDeltas[3], width, height);
    }

    private static Point MoveBlizzard(in Point p, in Delta direction, int width, int height)
    {
        int x = p.X + direction.X;
        if (x == -1)
        {
            x = width - 1;
        }
        if (x == width)
        {
            x = 0;
        }

        int y = p.Y + direction.Y;
        if (y == -1)
        {
            y = height - 1;
        }
        if (y == height)
        {
            y = 0;
        }

        return new(x, y);
    }

    private static bool AddPossibilities(in Point p, Dictionary<Point, BlizzardCell> blizzardMap, int width, int height, HashSet<Point> targetBuffer, in Point goal)
    {
        foreach (var direction in DirectionDeltas)
        {
            if (TryAddDelta(p, direction, blizzardMap, width, height, out Point target))
            {
                targetBuffer.Add(target);
                if (target == goal)
                {
                    return true;
                }
            }
        }

        if (!IsOutOfBounds(blizzardMap, width, height, p.X, p.Y) || p == new Point(0, -1) || p == new Point(width - 1, height))
        {
            // We could also stay put this turn
            targetBuffer.Add(p);
        }

        return false;
    }

    private static bool TryAddDelta(Point p, Delta direction, Dictionary<Point, BlizzardCell> blizzards, int width, int height, out Point target)
    {
        int x = p.X + direction.X;
        int y = p.Y + direction.Y;

        if (IsOutOfBounds(blizzards, width, height, x, y))
        {
            target = default;
            return false;
        }

        target = new(x, y);
        return true;
    }

    private static bool IsOutOfBounds(Dictionary<Point, BlizzardCell> blizzards, int width, int height, int x, int y)
    {
        return x < 0 || x >= width || y < 0 || y >= height || GetBlizzard(x, y, blizzards).HasBlizzard;
    }

    private static BlizzardCell GetBlizzard(int x, int y, Dictionary<Point, BlizzardCell> blizzards)
    {
        if (blizzards.TryGetValue(new(x, y), out BlizzardCell value))
        {
            return value;
        }

        // Return an empty cell
        return default;
    }

    private static void BuildBlizzards(ReadOnlySpan<string> lines, Dictionary<Point, BlizzardCell> blizzardMap)
    {
        for (int y = 1; y < lines.Length - 1; ++y)
        {
            for (int x = 1; x < lines[y].Length - 1; ++x)
            {
                TryAddBlizzard(lines, x, y, blizzardMap);

            }
        }
    }

    private static void TryAddBlizzard(ReadOnlySpan<string> lines, int x, int y, Dictionary<Point, BlizzardCell> blizzardMap)
    {
        switch (lines[y][x])
        {
            case '>':
                blizzardMap[new(x - 1, y - 1)] = new(Directions.Right);
                break;
            case 'v':
                blizzardMap[new(x - 1, y - 1)] = new(Directions.Down);
                break;
            case '<':
                blizzardMap[new(x - 1, y - 1)] = new(Directions.Left);
                break;
            case '^':
                blizzardMap[new(x - 1, y - 1)] = new(Directions.Up);
                break;
            default:
                break;
        };
    }
}

public enum State
{
    HeadingToEnd,
    TurningAround,
    HeadingToStart,
    TurningAroundAgain,
    HeadingToEndAgain,
}