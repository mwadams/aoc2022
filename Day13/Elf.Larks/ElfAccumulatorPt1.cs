namespace Elf.Larks;

using System;
using System.Collections.Generic;

internal readonly ref struct ElfAccumulatorPt1
{
    private readonly string[] lines;

    public ElfAccumulatorPt1(string[] lines)
    {
        this.lines = lines;
    }

    public int ProcessLines()
    {
        int count = 0;
        for (int i = 0; i < lines.Length; i += 3)
        {
            if (CompareLines(lines[i].AsSpan(), lines[i + 1].AsSpan()))
            {
                count += (i / 3) + 1;
            }
        }

        return count;
    }

    private bool CompareLines(ReadOnlySpan<char> lhs, ReadOnlySpan<char> rhs)
    {
        var (ValueLhs, _) = ParseArray(lhs);
        var (ValueRhs, _) = ParseArray(rhs);

        return ValueLhs.Compare(ValueRhs) == Status.InOrder;
    }

    private (Node Value, int Consumed) ParseArray(ReadOnlySpan<char> value)
    {
        List<Node> items = new();
        for (int i = 1; i < value.Length; ++i)
        {
            switch (value[i])
            {
                case ',':
                    continue;
                case ']':
                    return (new Node(items, false), i + 1);
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

        return (new Node(items, false), value.Length);
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