namespace Elf.Larks;

using System;
using System.Collections.Generic;

internal readonly ref struct ElfAccumulatorPt2
{
    private static readonly Node Divider1 = ParseArray("[[2]]").Value;
    private static readonly Node Divider2 = ParseArray("[[6]]").Value;

    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public readonly int ProcessLines()
    {
        Span<Node> orderedLines = new Node[((lines.Length + 1) * 2 / 3) + 2];
        BuildLines(this.lines, orderedLines);
        orderedLines.Sort(static (lhs, rhs) => (int)lhs.Compare(rhs));
        return CalculateResult(orderedLines);
    }

    private static void BuildLines(string[] lines, Span<Node> orderedLines)
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
    }

    private static int CalculateResult(ReadOnlySpan<Node> orderedLines)
    {
        int found = 0;
        int result = 1;
        for (int i = 0; i < orderedLines.Length; ++i)
        {
            Node value = orderedLines[i];
            if (IsDivider(value))
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

    private static bool IsDivider(Node value)
    {
        if (value.Children.Count == 1 &&
            value.Children[0].Children.Count == 1)
        {
            int? marker = value.Children[0].Children[0].Value;
            return marker is int m && (m == 6 || m == 2);
        }

        return false;
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
                        var (Value, Consumed) = ParseArray(value[i..]);
                        i += Consumed;
                        items.Add(Value);
                        break;
                    }
                default:
                    {
                        var (Value, Consumed) = ParseInteger(value[i..]);
                        i += Consumed;
                        items.Add(Value);
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