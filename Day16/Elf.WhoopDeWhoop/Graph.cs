namespace Elf.WhoopDeWhoop;

using System;
using System.Collections.Generic;

internal readonly ref struct Graph
{
    private readonly string[] lines;
    private readonly Dictionary<Node, int> flowRates = new();
    private readonly Dictionary<Node, int> flowNodeIndexes = new();
    private readonly Dictionary<PathKey, int> shortestPaths = new();
    // Pre-allocate the working set
    private readonly HashSet<VisitedKey> visited = new(200_000);

    public Graph(string[] lines)
    {
        this.lines = lines;
    }

    private int BuildGraph(Span<Node> allNodes, Span<Node> nodesWithFlow)
    {
        int allNodesCount = 0;
        int nodesWithFlowCount = 0;

        Span<Node> targetBuffer = stackalloc Node[10]; // No more than 10 edges per vertex
        foreach (var line in lines)
        {
            Node from = ParseName(line);
            int flowRate = ParseFlowRate(line);
            allNodes[allNodesCount++] = from;

            flowRates[from] = flowRate;
            if (flowRate > 0)
            {
                nodesWithFlow[nodesWithFlowCount++] = from;
                flowNodeIndexes[from] = nodesWithFlowCount;
            }

            // Set up Floyd-Warshall
            shortestPaths[new(from, from)] = 0;

            int targetCount = ParseDestinations(line, targetBuffer);
            foreach (Node to in targetBuffer[..targetCount])
            {
                shortestPaths[new(from, to)] = 1;
            }
        }

        // Execute Floyd-Warshall
        foreach (var nodeX in allNodes)
        {
            foreach (var node1 in allNodes)
            {
                foreach (var node2 in allNodes)
                {
                    PathKey node1Node2 = new(node1, node2);

                    shortestPaths[node1Node2] =
                        Math.Min(
                            shortestPaths.TryGetValue(node1Node2, out var v1) ? v1 : 1_000_000,
                            (shortestPaths.TryGetValue(new(node1, nodeX), out var v2) ? v2 : 1_000_000) +
                            (shortestPaths.TryGetValue(new(nodeX, node2), out var v3) ? v3 : 1_000_000));
                }
            }
        }

        return nodesWithFlowCount;
    }

    public T GraphSearch<T>(int minutes, Func<Item, T, T> onVisit, T initialValue)
    {
        Span<Node> allNodes = stackalloc Node[lines.Length];
        Span<Node> nodesWithFlowBuffer = stackalloc Node[lines.Length];
    
        int nodesWithFlowCount = BuildGraph(allNodes, nodesWithFlowBuffer);

        ReadOnlySpan<Node> nodesWithFlow = nodesWithFlowBuffer[..nodesWithFlowCount];

        int stackPointer = 0;
        Span<Item> stateStack = stackalloc Item[1024]; // Max stack depth.

        stateStack[stackPointer++] = new(Current: new('A', 'A'), RemainingTime: minutes, OpenedValves: 0, TotalFlow: 0);

        T value = initialValue;

        while (stackPointer > 0)
        {
            stackPointer--;
            Item item = stateStack[stackPointer];

            (long openedValves, int remainingTime, Node current, long totalFlow) = item;

            // We need to uniquely visit a node, at a particular time, with a particular set of valves opened
            // with a particular amount of preceding flow
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
                    if (!IsValveOpen(openedValves, nextIndex))
                    {
                        int nextRemaining = remainingTime - shortestPaths[new(current, next)] - 1;

                        if (nextRemaining > 0)
                        {
                            stateStack[stackPointer++] =
                                new(
                                    Current: next,
                                    OpenedValves: OpenValve(openedValves, nextIndex),
                                    RemainingTime: nextRemaining,
                                    TotalFlow: totalFlow + nextRemaining * flowRates[next]);
                        }
                    }
                }
            }
        }

        return value;
    }

    private static long OpenValve(long current, int nodeIndex)
    {
        long openMask = 1 << nodeIndex;
        return current | openMask;
    }

    private static bool IsValveOpen(long current, int nodeIndex)
    {
        long openMask = 1 << nodeIndex;
        long isOpenNum = current & openMask;
        return isOpenNum > 0;
    }

    private static int ParseDestinations(ReadOnlySpan<char> line, Span<Node> targets)
    {
        int targetCount = 0;
        int index = line.Length - 2;
        do
        {
            targets[targetCount++] = new(line[index], line[index + 1]);
            index -= 4;
        }
        while (line[index] >= 'A' && line[index] <= 'Z');

        return targetCount;
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
