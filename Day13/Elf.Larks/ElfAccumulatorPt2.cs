namespace Elf.Larks;

using System;
using System.Collections.Generic;

internal ref struct ElfAccumulatorPt2
{
    private static readonly Node Divider1 = ParseArray("[[2]]", true).Value;
    private static readonly Node Divider2 = ParseArray("[[6]]", true).Value;

    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public int ProcessLines()
    {
        Span<Node> orderedLines = new Node[lines.Length + 2];
        int written = BuildLines(this.lines, orderedLines);
        orderedLines = orderedLines[..written];
        Sort(orderedLines);
        return CalculateResult(orderedLines);
    }

    private static void Sort(Span<Node> orderedLines)
    {
        int foundMarkers = 0;

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
            // We can give up when both markers are at the bottom of a sort.
            if (orderedLines[length].IsMarker)
            {
                if (foundMarkers == 1)
                {
                    return;
                }

                foundMarkers++;

            }

        }
        while (length > 0);
    }

    private static int BuildLines(string[] lines, Span<Node> orderedLines)
    {
        int i = 0;
        orderedLines[i++] = Divider1;
        orderedLines[i++] = Divider2;

        foreach (var line in lines)
        {
            if (line.Length > 0)
            {
                orderedLines[i++] = ParseArray(line).Value;
            }
        }

        return i;
    }

    private static int CalculateResult(ReadOnlySpan<Node> orderedLines)
    {
        int found = 0;
        int result = 1;
        for (int i = 0; i < orderedLines.Length; ++i)
        {
            Node value = orderedLines[i];
            if (value.IsMarker)
            {
                if (found == 1)
                {
                    return result * (i + 1);
                }
                result *= i + 1;
                found++;
            }
        }

        throw new InvalidOperationException("Didn't find the boundary markers!");
    }

    private static bool CompareLines(Node lhs, Node rhs)
    {
        return lhs.Compare(rhs) == Status.InOrder;
    }

    private static (Node Value, int Consumed) ParseArray(ReadOnlySpan<char> value, bool isMarker = false)
    {
        List<Node> items = new();
        for (int i = 1; i < value.Length; ++i)
        {
            switch (value[i])
            {
                case ',':
                    continue;
                case ']':
                    return (new Node(items, isMarker), i + 1);
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

        return (new Node(items, isMarker), value.Length);
    }

    private static (Node Value, int Consumed) ParseInteger(ReadOnlySpan<char> value)
    {
        int index = 0;
        while (value[index] != ',' && value[index] != ']')
        {
            ++index;
        }

        return (new Node(int.Parse(value[..index]), false), index - 1);
    }
}