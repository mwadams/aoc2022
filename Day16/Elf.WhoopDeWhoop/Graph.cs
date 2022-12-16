namespace Elf.WhoopDeWhoop;

using System;
using System.Collections.Generic;

internal readonly ref struct Graph
{
    private readonly string[] lines;

    public Graph(string[] lines)
    {
        this.lines = lines;
    }

    private static int BuildGraph(string[] lines,
        Span<int> flowRates,
        Span<Node> allNodes,
        Span<Node> nodesWithFlowBuffer,
        Dictionary<Node, int> flowNodeIndexes,
        Dictionary<PathKey, int> shortestPaths)
    {
        int nodeCount = 0;
        int nodesWithFlowCount = 0;
        foreach (var line in lines)
        {
            Node from = ParseName(line);
            int flowRate = ParseFlowRate(line);
            allNodes[nodeCount] = from;
            flowRates[nodeCount++] = flowRate;
            if (flowRate > 0)
            {
                nodesWithFlowBuffer[nodesWithFlowCount] = from;
                flowNodeIndexes[from] = nodesWithFlowCount++;
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

        return nodesWithFlowCount;
    }

    public T GraphSearch<T>(int minutes, Func<Item, T, T> onVisit, T initialValue)
    {
        Span<int> flowRates = stackalloc int[lines.Length];
        Span<Node> allNodes = stackalloc Node[lines.Length];
        Span<Node> nodesWithFlowBuffer = stackalloc Node[lines.Length];
        Dictionary<Node, int> flowNodeIndexes = new();
        Dictionary<PathKey, int> shortestPaths = new();

        int flowNodeCount = BuildGraph(lines, flowRates, allNodes, nodesWithFlowBuffer, flowNodeIndexes, shortestPaths);
        ReadOnlySpan<Node> nodesWithFlow = nodesWithFlowBuffer[..flowNodeCount];

        Stack<Item> stateStack = new();
        stateStack.Push(new(Current: new('A', 'A'), RemainingTime: minutes, OpenedValves: 0, TotalFlow: 0));

        HashSet<VisitedKey> visited = new();

        T value = initialValue;

        while (stateStack.Count > 0)
        {
            Item item = stateStack.Pop()!;

            VisitedKey visitedKey = new(item.Current, item.RemainingTime, item.OpenedValves, item.TotalFlow);

            if (visited.Contains(visitedKey))
            {
                continue;
            }

            visited.Add(visitedKey);

            value = onVisit(item, value);

            if (item.RemainingTime != 0)
            {
                foreach (Node next in nodesWithFlow)
                {
                    int nextIndex = flowNodeIndexes[next];
                    if (!IsOpen(item.OpenedValves, nextIndex))
                    {
                        int nextRemaining = item.RemainingTime - shortestPaths[new(item.Current, next)] - 1;

                        if (nextRemaining > 0)
                        {
                            stateStack.Push(
                                new(
                                    Current: next,
                                    OpenedValves: AddOpen(item.OpenedValves, nextIndex),
                                    RemainingTime: nextRemaining,
                                    TotalFlow: item.TotalFlow + nextRemaining * flowRates[nextIndex]));
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
