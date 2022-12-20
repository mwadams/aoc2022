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
                indexOfZero = i;
                break;
            }
        }

        int index1000 = (1000 + indexOfZero) % positions.Length;
        int index2000 = (2000 + indexOfZero) % positions.Length;
        int index3000 = (3000 + indexOfZero) % positions.Length;

        return positions[index1000].Value  + positions[index2000].Value + positions[index3000].Value;
    }

    private static void Mix(Span<(int Value, int Index)> positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            int currentIndex = FindOriginalItemIndex(positions, i);
            int value = positions[currentIndex].Value;

            int targetIndex = GetTargetFor(currentIndex, value, positions.Length - 1);
            if (currentIndex == targetIndex)
            {
                // Nothing to do if it doesn't move.
                continue;
            }

            Move(positions, currentIndex, targetIndex);
        }
    }

    private static void Move(Span<(int Value, int Index)> positions, int from, int to)
    {
        (int, int) tmp = positions[from];
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

    private static int FindOriginalItemIndex(Span<(int Value, int Index)> positions, int index)
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

    private static void Write(Span<(int Value, int Index)> positions, string? append = null)
    {
        foreach (var item in positions)
        {
            Console.CursorLeft = item.Index;
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
        if (!string.IsNullOrEmpty(append))
        {
            Console.Write(' ');
            Console.Write(append);
        }

        Console.ReadLine();
    }

    private static int GetTargetFor(int index, int amountToMove, int modVal)
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

        int target = index + amountToMove;

        if (target >= modVal)
        {
            target -= modVal;
        }

        return target;
    }

    private static void ProcessLine(ReadOnlySpan<char> line, int index, Span<(int, int)> message)
    {
        message[index] = (int.Parse(line), index);
    }
}