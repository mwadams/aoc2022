﻿namespace Elf.Larks;

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
        bool found1 = false;
        bool found2 = false;
        int result = 1;
        for (int i = 0; i < orderedLines.Length; ++i)
        {
            Node value = orderedLines[i];
            if (!found1 && value.Is(Divider1))
            {
                if (found2)
                {
                    return result * (i + 1);
                }
                result *= i + 1;
                found1 = true; ;
            }

            if (!found2 && value.Is(Divider2))
            {
                if (found1)
                {
                    return result * (i + 1);
                }
                result *= i + 1;
                found2 = true; ;
            }
        }

        throw new InvalidOperationException("Didn't find the boundary markers!");
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