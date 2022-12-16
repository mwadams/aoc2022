namespace Elf.WhoopDeWhoop;

using System;
using System.Collections.Generic;

internal readonly ref struct Graph
{
    private readonly string[] lines;
    private readonly Dictionary<Node, int> flowRates = new();
    private readonly List<Node> allNodes = new();
    private readonly List<Node> nodesWithFlow = new();
    private readonly Dictionary<Node, int> flowNodeIndexes = new();
    private readonly Dictionary<PathKey, int> shortestPaths = new();

    public Graph(string[] lines)
    {
        this.lines = lines;
    }

    private void BuildGraph()
    {
        foreach (var line in lines)
        {
            Node from = ParseName(line);
            int flowRate = ParseFlowRate(line);
            allNodes.Add(from);
            flowRates[from] = flowRate;
            if (flowRate > 0)
            {
                nodesWithFlow.Add(from);
                flowNodeIndexes[from] = nodesWithFlow.Count;
            }

            // Set up Floyd-Warshall
            shortestPaths[new(from, from)] = 0;

            foreach (Node to in ParseDestinations(line))
            {
                shortestPaths[new(from, to)] = 1;
            }
        }

        // Execute Floyd-Warshall
        foreach (var hopNode in allNodes)
        {
            foreach (var node1 in allNodes)
            {
                foreach (var node2 in allNodes)
                {
                    PathKey node1Node2 = new(node1, node2);

                    shortestPaths[node1Node2] =
                        Math.Min(
                            shortestPaths.TryGetValue(node1Node2, out var v1) ? v1 : 1_000_000,
                            (shortestPaths.TryGetValue(new(node1, hopNode), out var v2) ? v2 : 1_000_000) +
                            (shortestPaths.TryGetValue(new(hopNode, node2), out var v3) ? v3 : 1_000_000));
                }
            }
        }
    }

    public T GraphSearch<T>(int minutes, Func<Item, T, T> onVisit, T initialValue)
    {
        BuildGraph();

        Stack<Item> stateStack = new();
        stateStack.Push(new(Current: new('A', 'A'), RemainingTime: minutes, OpenedValves: 0, TotalFlow: 0));

        HashSet<VisitedKey> visited = new();

        T value = initialValue;

        while (stateStack.Count > 0)
        {
            Item item = stateStack.Pop()!;

            (long openedValves, int remainingTime, Node current, long totalFlow) = item;

            VisitedKey visitedKey = new(current, remainingTime, openedValves, totalFlow);

            if (visited.Contains(visitedKey))
            {
                continue;
            }

            visited.Add(visitedKey);

            value = onVisit(item, value);

            if (remainingTime != 0)
            {
                foreach (Node next in nodesWithFlow)
                {
                    int nextIndex = flowNodeIndexes[next];
                    if (!IsOpen(openedValves, nextIndex))
                    {
                        int nextRemaining = remainingTime - shortestPaths[new(current, next)] - 1;

                        if (nextRemaining > 0)
                        {
                            stateStack.Push(
                                new(
                                    Current: next,
                                    OpenedValves: AddOpen(openedValves, nextIndex),
                                    RemainingTime: nextRemaining,
                                    TotalFlow: totalFlow + nextRemaining * flowRates[next]));
                        }
                    }
                }
            }
        }

        return value;
    }

    private static long AddOpen(long current, int nodeIndex)
    {
        long openMask = 1 << nodeIndex;
        return current | openMask;
    }

    private static bool IsOpen(long current, int nodeIndex)
    {
        long openMask = 1 << nodeIndex;
        // Stupid "prettier" won't let me inline this code and puts bad parentheses!
        long isOpenNum = current & openMask;
        return isOpenNum > 0;
    }

    private static List<Node> ParseDestinations(ReadOnlySpan<char> line)
    {
        List<Node> targets = new();

        int index = line.Length - 2;
        do
        {
            targets.Add(new(line[index], line[index + 1]));
            index -= 4;
        }
        while (line[index] >= 'A' && line[index] <= 'Z');

        return targets;
    }

    private static int ParseFlowRate(ReadOnlySpan<char> line)
    {
        int endIndex = line[24..].IndexOf(';') + 24;
        return int.Parse(line[23..endIndex]);
    }

    private static Node ParseName(ReadOnlySpan<char> line)
    {
        return new Node(line[6], line[7]);
    }
}
