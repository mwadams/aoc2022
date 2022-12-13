namespace Elf.Larks;

using System;
using System.Collections.Generic;

internal ref struct ElfAccumulatorPt2
{
    private const string Divider1 = "[[2]]";
    private const string Divider2 = "[[6]]";

    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public int ProcessLines()
    {
        Span<string> orderedLines = new string[lines.Length + 2];
        int written = BuildLines(this.lines, orderedLines);
        orderedLines = orderedLines[..written];
        Sort(orderedLines);
        return CalculateResult(orderedLines);
    }

    private static void Sort(Span<string> orderedLines)
    {
        // Bubblesort
        int length = orderedLines.Length;
        do
        {
            int newLength = 0;
            for (int i = 1; i < length; ++i)
            {
                if (!CompareLines(orderedLines[i - 1], orderedLines[i]))
                {
                    // Use tuple to swap values
                    (orderedLines[i], orderedLines[i - 1]) = (orderedLines[i - 1], orderedLines[i]);
                    newLength = i;
                }
            }

            length = newLength;
        }
        while (length > 0);
    }

    private static int BuildLines(string[] lines, Span<string> orderedLines)
    {
        int i = 0;
        foreach (var line in lines)
        {
            if (line.Length > 0)
            {
                orderedLines[i++] = line;
            }
        }

        orderedLines[i++] = Divider1;
        orderedLines[i++] = Divider2;
        return i;
    }

    private static int CalculateResult(ReadOnlySpan<string> orderedLines)
    {
        bool found1 = false;
        bool found2 = false;

        int result = 1;
        for (int i = 0; i < orderedLines.Length; ++i)
        {
            string value = orderedLines[i];
            if (value == Divider1)
            {
                if (found2)
                {
                    return result * (i + 1);
                }
                result *= i + 1;
                found1 = true;
            }

            if (value == Divider2)
            {
                if (found1)
                {
                    return result * (i + 1);
                }
                result *= i + 1;
                found2 = true;
            }
        }

        throw new InvalidOperationException("Didn't find the boundary markers!");
    }

    private static bool CompareLines(ReadOnlySpan<char> lhs, ReadOnlySpan<char> rhs)
    {
        var lhsResult = ParseArray(lhs);
        var rhsResult = ParseArray(rhs);

        return lhsResult.Value.Compare(rhsResult.Value) == Status.InOrder;
    }

    private static (Node Value, int Consumed) ParseArray(ReadOnlySpan<char> value)
    {
        List<Node> items = new();
        for (int i = 1; i < value.Length; ++i)
        {
            switch (value[i])
            {
                case ',':
                    continue;
                case ']':
                    return (new Node(items), i + 1);
                case '[':
                    {
                        var result = ParseArray(value[i..]);
                        i += result.Consumed;
                        items.Add(result.Value);
                        break;
                    }
                default:
                    {
                        var result = ParseInteger(value[i..]);
                        i += result.Consumed;
                        items.Add(result.Value);
                        break;
                    }
            }
        }

        return (new Node(items), value.Length);
    }

    private static (Node Value, int Consumed) ParseInteger(ReadOnlySpan<char> value)
    {
        int index = 0;
        while (value[index] != ',' && value[index] != ']')
        {
            ++index;
        }

        return (new Node(int.Parse(value[..index])), index - 1);
    }
}