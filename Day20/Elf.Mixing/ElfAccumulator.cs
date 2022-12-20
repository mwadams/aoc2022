namespace Elf.Mixing;

public readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public int Process()
    {
        int index = 0;
        Span<(int Value, int Index)> positions = stackalloc (int, int)[lines.Length];
        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan(), index++, positions);
        }

        Mix(positions);

        int indexOfZero = 0;

        for (int i = 0; i < positions.Length; ++i)
        {
            if (positions[i].Value == 0)
            {
                indexOfZero = positions[i].Index;
                break;
            }
        }

        int index1000 = (1000 + indexOfZero) % positions.Length;
        int index2000 = (2000 + indexOfZero) % positions.Length;
        int index3000 = (3000 + indexOfZero) % positions.Length;

        FindValues(positions, index1000, index2000, index3000, out int val1000, out int val2000, out int val3000);

        return val1000 + val2000 + val3000;
    }

    private static void FindValues(Span<(int Value, int Index)> positions, int index1000, int index2000, int index3000, out int val1000, out int val2000, out int val3000)
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

    private static void Mix(Span<(int Value, int Index)> positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].Value == 0)
            {
                Write(positions, "No change");
                continue;
            }

            int target = GetTargetFor(ref positions[i], positions.Length - 1);
            if (positions[i].Index == target)
            {
                // Nothing to do if it doesn't move.
                Write(positions, "No change");
                continue;
            }


            for (int j = 0; j < positions.Length; ++j)
            {
                Update(ref positions[j], positions[i].Index, target);
            }

            positions[i] = (positions[i].Value, target);
            Write(positions);
        }
    }

    private static void Write(Span<(int Value, int Index)> positions, string? append = null)
    {
        //foreach (var item in positions)
        //{
        //    Console.CursorLeft = item.Index;
        //    if (item.Value < 0)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //    }
        //    else
        //    {
        //        Console.ForegroundColor = ConsoleColor.Green;
        //    }

        //    Console.Write(Math.Abs(item.Value));
        //}
        //if (!string.IsNullOrEmpty(append))
        //{
        //    Console.Write(' ');
        //    Console.Write(append);
        //}

        //Console.ReadLine();
    }

    private static int GetTargetFor(ref (int Value, int Index) item, int modVal)
    {
        int amountToMove = item.Value;
        if (amountToMove < 0)
        {
            amountToMove = -amountToMove % modVal;
            amountToMove = modVal - amountToMove;
        }
        else
        {
            amountToMove %= modVal;
        }

        int target = item.Index + amountToMove;

        if (target >= modVal)
        {
            target -= modVal;
        }

        return target;
    }

    private static void Update(ref (int Value, int Index) item, int from, int to)
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

    private static void ProcessLine(ReadOnlySpan<char> line, int index, Span<(int, int)> message)
    {
        message[index] = (int.Parse(line), index);
    }
}