namespace Elf.MonkeyMap;

using System;

internal static class MonkeyWalk2
{
    private static readonly Delta[] movementDeltas = new Delta[] { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };
    private static readonly PossibleConnection[] possibleConnections = new PossibleConnection[]
    {
        new(0, new Connection[] { new(1, -1), new(3, 1) }),
        new(1, new Connection[] { new(0, 1), new(2, -1) }),
        new(2, new Connection[] { new(1, 1), new(3, -1) }),
        new(3, new Connection[] { new(0, -1), new(2, 1) }),
    };

    private static ReadOnlySpan<Delta> MovementDeltas => movementDeltas.AsSpan();
    private static ReadOnlySpan<PossibleConnection> PossibleConnections => possibleConnections.AsSpan();

    // This is the search pattern from a face to find its adjacent face

    public static void BuildMap(ReadOnlySpan<string> lines, out ReadOnlySpan<char> instructions, out int mapHeight, out int mapWidth)
    {
        instructions = ReadOnlySpan<char>.Empty;
        mapHeight = 0;
        mapWidth = 0;

        bool foundInstructions = false;
        foreach (var line in lines)
        {
            if (foundInstructions)
            {
                instructions = line;
                break;
            }

            if (line.Length == 0)
            {
                foundInstructions = true;
                continue;
            }

            mapWidth = Math.Max(line.Length, mapWidth);
            mapHeight++;
        }


        if (!foundInstructions)
        {
            throw new InvalidOperationException();
        }
    }

    public static long GoForAWalk(ReadOnlySpan<string> map, ReadOnlySpan<char> instructions, int mapWidth, int mapHeight)
    {
        int cubeSize = Math.Abs(mapWidth - mapHeight);

        Span<int> cube = stackalloc int[16];

        BuildCube(map, mapWidth, mapHeight, cubeSize, cube);

        Span<Connection> connections = stackalloc Connection[28];

        BuildConnections(cube, connections);

        int x = FindInitialX(map[0]), y = 0;

        int direction = 0;
        int walkIndex = 0;

        while (walkIndex < instructions.Length)
        {
            int cellX = x / cubeSize;
            int cellY = y / cubeSize;
            int face = cube[cellX + cellY * 4];

            if (instructions[walkIndex] == 'L')
            {
                direction = (direction + 3) % 4;
                walkIndex++;
            }
            else if (instructions[walkIndex] == 'R')
            {
                direction = (direction + 1) % 4;
                walkIndex++;
            }
            else
            {
                walkIndex += ParseDistanceToWalk(instructions[walkIndex..], out int steps);
                for (int j = 0; j < steps; j++)
                {
                    var delta = MovementDeltas[direction];
                    int newX = x + delta.X;
                    int newY = y + delta.Y;
                    int newDirection = direction;

                    int newCubeX = (int)Math.Floor(newX / (double)cubeSize);
                    int newCubeY = (int)Math.Floor(newY / (double)cubeSize);
                    int movementIndex = MovementDeltas.IndexOf(new Delta(newCubeX - cellX, newCubeY - cellY));
                    if (movementIndex != -1)
                    {
                        var connection = connections[face + (movementIndex * 7)];
                        int faceIndex = cube.IndexOf(connection.Face);
                        int cubeX = faceIndex % 4;
                        int cubeY = faceIndex / 4;
                        int transformedX = (newX + cubeSize) % cubeSize - cubeSize / 2;
                        int transformedY = (newY + cubeSize) % cubeSize - cubeSize / 2;
                        switch (connection.Rotation)
                        {
                            case 0:
                                break;
                            case 1:
                                newDirection = (direction + 1) % 4;
                                (transformedX, transformedY) = (-transformedY - 1, transformedX);
                                break;
                            case 2:
                                newDirection = (direction + 2) % 4;
                                (transformedX, transformedY) = (-transformedX - 1, -transformedY - 1);
                                break;
                            case 3:
                                newDirection = (direction + 3) % 4;
                                (transformedX, transformedY) = (transformedY, -transformedX - 1);
                                break;
                        }

                        transformedX += cubeSize / 2;
                        transformedY += cubeSize / 2;
                        newX = (cubeX * cubeSize) + transformedX;
                        newY = (cubeY * cubeSize) + transformedY;
                    }

                    if (map.GetPointOrBlank(newX, newY) != '#')
                    {
                        x = newX;
                        y = newY;
                        direction = newDirection;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return (y + 1) * 1000 + (x + 1) * 4 + direction;
    }

    private static void BuildConnections(ReadOnlySpan<int> cube, Span<Connection> connections)
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                int face = cube[x + y * 4];
                if (face != 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var delta = MovementDeltas[i];
                        int newX = (x + delta.X + 4) % 4;
                        int newY = (y + delta.Y + 4) % 4;
                        int target = cube[newX + (newY * 4)];
                        if (target != 0)
                        {
                            connections[face + (i * 7)] = new(target, 0);
                        }
                    }
                }
            }
        }

        for (int directionIndex = 0; directionIndex < 3; directionIndex++)
        {
            for (int faceIndex = 1; faceIndex <= 6; faceIndex++)
            {
                foreach (var possibleConnection in PossibleConnections)
                {
                    var connectedFace = connections[faceIndex + (possibleConnection.Direction * 7)];
                    if (connectedFace.Face == 0)
                    {
                        continue;
                    }

                    int face = connectedFace.Face;
                    foreach (var connection in possibleConnection.Connection)
                    {
                        if (connections[faceIndex + (connection.Face * 7)].Face != 0)
                        {
                            continue;
                        }

                        var target = connections[face + (((connection.Face + connectedFace.Rotation) % 4) * 7)];

                        bool foundTarget = false;

                        for (int side = 0; side < 4; side++)
                        {
                            if (connections[faceIndex + (side * 7)].Face == target.Face)
                            {
                                foundTarget = true;
                                break;
                            }
                        }

                        if (!foundTarget)
                        {
                            if (target.Face != 0 && faceIndex != target.Face)
                            {
                                int rotation = (connectedFace.Rotation + target.Rotation + connection.Rotation + 4) % 4;
                                connections[faceIndex + (connection.Face * 7)] = new(target.Face, rotation);
                            }
                        }
                    }
                }
            }
        }
    }

    private static void BuildCube(ReadOnlySpan<string> map, int mapWidth, int mapHeight, int cubeSize, Span<int> cube)
    {
        int face = 0;
        for (int y = 0; y < mapHeight; y += cubeSize)
        {
            for (int x = 0; x < mapWidth; x += cubeSize)
            {
                if (map.GetPointOrBlank(x, y) != (char)0)
                {
                    face++;
                    cube[x / cubeSize + (y / cubeSize) * 4] = face;
                }
            }
        }
    }

    // It returns the length of the integer string to advance the counter
    private static int ParseDistanceToWalk(ReadOnlySpan<char> instructionFragment, out int distanceToWalk)
    {
        int i = 0;
        while (i < instructionFragment.Length && instructionFragment[i] >= '0' && instructionFragment[i] <= '9')
        {
            ++i;
        }

        distanceToWalk = int.Parse(instructionFragment[0..i]);
        return i;
    }

    private static int FindInitialX(ReadOnlySpan<char> line)
    {
        for (int i = 0; i < line.Length; ++i)
        {
            if (line[i] == '.')
            {
                return i;
            }
        }

        throw new InvalidOperationException();
    }

    private static char GetPointOrBlank(this ReadOnlySpan<string> map, int x, int y)
    {
        if (y >= 0 && y < map.Length && x >= 0 && x < map[y].Length && map[y][x] != ' ')
        {
            return map[y][x];
        }

        return default;
    }
}

internal readonly record struct Connection(int Face, int Rotation);
internal readonly record struct Delta(int X, int Y);
internal readonly record struct PossibleConnection(int Direction, Connection[] Connection);