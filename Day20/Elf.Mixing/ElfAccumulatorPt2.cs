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
        const int decryptionKey = 811589153;
        const int iterations = 10;

        int index = 0;
        Span<(long Value, int Index)> positions = stackalloc (long, int)[lines.Length];
        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan(), index++, positions, decryptionKey);
        }

        for(int i = 0; i < iterations; ++i)
        {
            Mix(positions);
        }

        int indexOfZero = 0;

        for (int i = 0; i < positions.Length; ++i)
        {
            if (positions[i].Value == 0)
            {
                indexOfZero = i;
                break;
            }
        }

        int index1000 = (1000 + indexOfZero) % positions.Length;
        int index2000 = (2000 + indexOfZero) % positions.Length;
        int index3000 = (3000 + indexOfZero) % positions.Length;

        return positions[index1000].Value + positions[index2000].Value + positions[index3000].Value;
    }

    private static void Mix(Span<(long Value, int Index)> positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            int currentIndex = FindOriginalItemIndex(positions, i);
            long value = positions[currentIndex].Value;

            int targetIndex = GetTargetFor(currentIndex, value, positions.Length - 1);
            if (currentIndex == targetIndex)
            {
                continue;
            }

            Move(positions, currentIndex, targetIndex);
        }
    }

    private static void Move(Span<(long Value, int Index)> positions, int from, int to)
    {
        (long, int) tmp = positions[from];
        int length = from - to;

        if (length > 0)
        {
            positions.Slice(to, length).CopyTo(positions.Slice(to + 1, length));
        }
        else if (length < 0)
        {
            positions.Slice(from + 1, -length).CopyTo(positions.Slice(from, -length));
        }

        positions[to] = tmp;
    }

    private static int FindOriginalItemIndex(Span<(long Value, int Index)> positions, int index)
    {
        for (int i = 0; i < positions.Length; ++i)
        {
            if (positions[i].Index == index)
            {
                return i;
            }
        }

        return -1;
    }

    private static void Write(Span<(long Value, int Index)> positions)
    {
        bool first = true;
        foreach (var item in positions)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                Console.Write(',');
            }

            if (item.Value < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.Write(Math.Abs(item.Value));
        }

        Console.ReadLine();
    }

    private static int GetTargetFor(int index, long amountToMove, int modVal)
    {
        if (amountToMove < 0)
        {
            amountToMove = -amountToMove % modVal;
            amountToMove = modVal - amountToMove;
        }
        else
        {
            amountToMove %= modVal;
        }

        long target = index + amountToMove;

        if (target >= modVal)
        {
            target -= modVal;
        }

        return (int)target;
    }

    private static void ProcessLine(ReadOnlySpan<char> line, int index, Span<(long, int)> message, long decryptionKey)
    {
        message[index] = (int.Parse(line) * decryptionKey, index);
    }
}