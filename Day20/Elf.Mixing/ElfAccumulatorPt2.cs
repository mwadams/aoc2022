namespace Elf.Mixing;

public readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        int decryptionKey = 811589153;
        int index = 0;

        Span<(long Value, long Index)> positions = stackalloc (long, long)[lines.Length];
        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan(), index++, positions);
        }

        for (int i = 0; i < positions.Length; ++i)
        {
            positions[i].Value *= decryptionKey;
        }

        for (int i = 0; i < 10; ++i)
        {
            Mix(positions);
        }

        long indexOfZero = 0;

        for (int i = 0; i < positions.Length; ++i)
        {
            if (positions[i].Value == 0)
            {
                indexOfZero = positions[i].Index;
                break;
            }
        }

        long index1000 = (1000 + indexOfZero) % positions.Length;
        long index2000 = (2000 + indexOfZero) % positions.Length;
        long index3000 = (3000 + indexOfZero) % positions.Length;
        
        FindValues(positions, index1000, index2000, index3000, out long val1000, out long val2000, out long val3000);

        return (val1000 + val2000 + val3000);
    }

    private static void FindValues(Span<(long Value, long Index)> positions, long index1000, long index2000, long index3000, out long val1000, out long val2000, out long val3000)
    {
        bool found1 = false;
        bool found2 = false;
        bool found3 = false;

        val1000 = 0;
        val2000 = 0;
        val3000 = 0;


        for (int i = 0; i < positions.Length; ++i)
        {
            if (!found1 && positions[i].Index == index1000)
            {
                found1 = true;
                val1000 = positions[i].Value;
                if (found2 && found3)
                {
                    return;
                }
            }

            if (!found2 && positions[i].Index == index2000)
            {
                found2 = true;
                val2000 = positions[i].Value;
                if (found1 && found3)
                {
                    return;
                }
            }

            if (!found3 && positions[i].Index == index3000)
            {
                found3 = true;
                val3000 = positions[i].Value;
                if (found1 && found2)
                {
                    return;
                }
            }
        }

        throw new InvalidOperationException();
    }

    private static void Mix(Span<(long Value, long Index)> positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].Value == 0)
            {
                continue;
            }

            long target = GetTargetFor(ref positions[i], positions.Length - 1);
            if (positions[i].Index == target)
            {
                continue;
            }


            for (int j = 0; j < positions.Length; ++j)
            {
                Update(ref positions[j], positions[i].Index, target);
            }

            positions[i] = (positions[i].Value, target);
        }
    }

    private static void DumpToConsole(Span<(long Value, long Index)> positions)
    {
        Span<(long Value, long Index)> copy = stackalloc (long, long)[positions.Length];
        positions.CopyTo(copy);
        copy.Sort((l, r) => Math.Sign(l.Index - r.Index));

        foreach (var item in copy)
        {
            if (item.Value < 0)
            {
                Console.ForegroundColor = ConsoleColor
                    .Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.Write(Math.Abs(item.Value));
            Console.Write(',');
        }

        Console.ReadLine();
    }

    private static long GetTargetFor(ref (long Value, long Index) item, long modVal)
    {
        long amountToMove = item.Value;
        if (amountToMove < 0)
        {
            amountToMove = -amountToMove % modVal;
            amountToMove = modVal - amountToMove;
        }
        else
        {
            amountToMove %= modVal;
        }

        long target = item.Index + amountToMove;

        if (target >= modVal)
        {
            target -= modVal;
        }

        return target;
    }

    private static void Update(ref (long Value, long Index) item, long from, long to)
    {
        if (to > from)
        {
            if (item.Index <= to && item.Index > from)
            {
                item.Index -= 1;
            }
        }
        else
        {
            if (item.Index < from && item.Index >= to)
            {
                item.Index += 1;
            }
        }
    }

    private static void ProcessLine(ReadOnlySpan<char> line, int index, Span<(long, long)> message)
    {
        message[index] = (long.Parse(line), index);
    }
}