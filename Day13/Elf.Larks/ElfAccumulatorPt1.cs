namespace Elf.Larks;

using System;
using System.Collections.Generic;

internal ref struct ElfAccumulatorPt1
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
        var lhsResult = ParseArray(lhs);
        var rhsResult = ParseArray(rhs);

        return lhsResult.Value.Compare(rhsResult.Value) == Status.InOrder;
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

    private (Node Value, int Consumed) ParseInteger(ReadOnlySpan<char> value)
    {
        int index = 0;
        while (value[index] != ',' && value[index] != ']')
        {
            ++index;
        }

        return (new Node(int.Parse(value[..index])), index - 1);
    }
}