namespace Elf.Monkeys;

using System;
using System.Collections.Generic;
using System.Numerics;

internal ref struct ElfAccumulator
{
    private readonly string[] instructions;

    public ElfAccumulator(string[] instructions)
    {
        this.instructions = instructions;
    }

    public ulong MonkeyBusiness { get; private set; }

    public void Play(int rounds, int topN, bool isPart1)
    {
        List<Monkey> monkeys = this.ParseMonkeys();

        Func<ulong, ulong> applyRelief = GetReliefFunction(isPart1, monkeys);

        Span<int> inspectionCount = stackalloc int[monkeys.Count];

        for (int round = 0; round < rounds; ++round)
        {
            PlayRound(monkeys, inspectionCount, applyRelief);
        }

        inspectionCount.Sort();
        this.MonkeyBusiness = 1;

        for (int i = 0; i < topN; ++i)
        {
            this.MonkeyBusiness *= (ulong)inspectionCount[^(i + 1)];
        }
    }

    private static Func<ulong, ulong> GetReliefFunction(bool part1, List<Monkey> monkeys)
    {
        Func<ulong, ulong> applyRelief;
        if (part1)
        {
            applyRelief = worry => worry / 3;
        }
        else
        {
            ulong lowestCommonMultiple = part1 ? 0 : CalculateLowestCommonMultiple(monkeys);
            applyRelief = worry => worry % lowestCommonMultiple;
        }

        return applyRelief;
    }

    private static ulong CalculateLowestCommonMultiple(List<Monkey> monkeys)
    {
        ulong result = 1;
        foreach (var monkey in monkeys)
        {
            result = result * monkey.TestValue;
        }

        return result;
    }

    private static void PlayRound(List<Monkey> monkeys, Span<int> inspectionCount, Func<ulong, ulong> applyRelief)
    {
        int monkeyIndex = 0;
        foreach (Monkey monkey in monkeys)
        {
            foreach (ulong item in monkey.Items)
            {
                inspectionCount[monkeyIndex]++;

                ulong worryLevel = monkey.Operation(item);

                worryLevel = applyRelief(worryLevel);

                if (monkey.Test(worryLevel))
                {
                    monkeys[monkey.TrueTarget].Items.Add(worryLevel);
                }
                else
                {
                    monkeys[monkey.FalseTarget].Items.Add(worryLevel);
                }
            }

            // We gave away all the items
            monkey.Items.Clear();
            monkeyIndex++;
        }
    }

    private List<Monkey> ParseMonkeys()
    {
        List<Monkey> monkeys = new();

        // Blocks of 7 for each monkey
        for (int i = 0; i < this.instructions.Length; i += 7)
        {
            ulong testValue = ParseTestValue(this.instructions[i + 3].AsSpan());

            monkeys.Add(new(
                ParseItems(this.instructions[i + 1].AsSpan()),
                ParseOperation(this.instructions[i + 2].AsSpan()),
                value => value % testValue == 0,
                testValue,
                ParseTrueTarget(this.instructions[i + 4].AsSpan()),
                ParseFalseTarget(this.instructions[i + 5].AsSpan())));
        }

        return monkeys;
    }

    private static int ParseFalseTarget(ReadOnlySpan<char> line)
    {
        return int.Parse(line[(line.LastIndexOf(' ') + 1)..]);
    }

    private static int ParseTrueTarget(ReadOnlySpan<char> line)
    {
        return int.Parse(line[(line.LastIndexOf(' ') + 1)..]);
    }

    private static ulong ParseTestValue(ReadOnlySpan<char> line)
    {
        return uint.Parse(line[(line.LastIndexOf(' ') + 1)..]);
    }

    private static Func<ulong, ulong> ParseOperation(ReadOnlySpan<char> line)
    {
        int lastSpace = line.LastIndexOf(' ');
        bool addIfTrueMultiplyIfFalse = line[lastSpace - 1] == '+';
        if (line[lastSpace + 1] == 'o')
        {
            if (addIfTrueMultiplyIfFalse)
            {
                return old => old + old;
            }
            else
            {
                return old => old * old;
            }
        }
        else
        {

            uint value = uint.Parse(line[(lastSpace + 1)..]);
            if (addIfTrueMultiplyIfFalse)
            {
                return old => old + value;
            }
            else
            {
                return old => old * value;
            }
        }
    }

    private static List<ulong> ParseItems(ReadOnlySpan<char> line)
    {
        List<ulong> items = new();

        int index = line.IndexOf(':') + 2;
        while (true)
        {
            int nextIndex = line[index..].IndexOf(',');
            if (nextIndex == -1)
            {
                items.Add((ulong)uint.Parse(line[index..]));
                break;
            }

            items.Add((ulong)uint.Parse(line[index..(index + nextIndex)]));
            index += nextIndex + 1;
        }

        return items;
    }
}