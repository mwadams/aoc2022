namespace Elf.MonkeyMap;

using System;
using System.Security.AccessControl;

internal static class MonkeyWalk
{
    public static void BuildMap(ReadOnlySpan<string> lines, out ReadOnlySpan<char> instructions, out int mapHeight)
    {
        instructions = ReadOnlySpan<char>.Empty;
        mapHeight = 0;

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

            mapHeight++;
        }


        if (!foundInstructions)
        {
            throw new InvalidOperationException();
        }
    }

    private enum Direction { Right = 0, Down = 1, Left = 2, Up = 3 };

    public static long GoForAWalk(ReadOnlySpan<string> map, ReadOnlySpan<char> instructions)
    {
        int walkIndex = 0;
        int mapX = FindInitialX(map[0]);
        int mapY = 0;
        Direction direction = Direction.Right;

        while (walkIndex < instructions.Length)
        {
            if (instructions[walkIndex] == 'R')
            {
                direction = NextDirectionClockwise(direction);
                walkIndex++;
            }
            else if (instructions[walkIndex] == 'L')
            {
                direction = NextDirectionAnticlockwise(direction);
                walkIndex++;
            }
            else
            {
                walkIndex += ParseDistanceToWalk(instructions[walkIndex..], out int distanceToWalk);
                Walk(map, ref mapX, ref mapY, distanceToWalk, direction);
            }
        }

        return (1000 * (mapY + 1)) + (4 * (mapX + 1)) + (long)direction;
    }

    // It returns the lenght of the integer string
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

    private static void Walk(ReadOnlySpan<string> map, ref int mapX, ref int mapY, int distanceToWalk, Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                WalkRight(map, ref mapX, ref mapY, distanceToWalk);
                break;
            case Direction.Down:
                WalkDown(map, ref mapX, ref mapY, distanceToWalk);
                break;
            case Direction.Left:
                WalkLeft(map, ref mapX, ref mapY, distanceToWalk);
                break;
            case Direction.Up:
                WalkUp(map, ref mapX, ref mapY, distanceToWalk);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    private static void WalkRight(ReadOnlySpan<string> map, ref int mapX, ref int mapY, int distanceToWalk)
    {
        ReadOnlySpan<char> currentLine = map[mapY];
        for (int i = 0; i < distanceToWalk; ++i)
        {
            if (mapX + 1 >= currentLine.Length || currentLine[mapX + 1] == ' ')
            {
                if (WrapRightToLeft(currentLine, ref mapX))
                {
                    // we had to stop
                    break;
                }

                // We reappeared on the other edge
                continue;
            }

            if (currentLine[mapX + 1] == '#')
            {
                break;
            }

            // Take a step
            mapX++;
        }
    }

    private static bool WrapRightToLeft(ReadOnlySpan<char> currentLine, ref int mapX)
    {
        int offset = -1;

        // We look left until we find the space
        while (mapX + offset >= 0 && currentLine[mapX + offset] != ' ')
        {
            offset--;
        }

        // This is the square to look at.
        offset++;
        if (currentLine[mapX + offset] == '#')
        {
            return true;
        }

        mapX += offset;
        return false;
    }

    private static void WalkLeft(ReadOnlySpan<string> map, ref int mapX, ref int mapY, int distanceToWalk)
    {
        ReadOnlySpan<char> currentLine = map[mapY];
        for (int i = 0; i < distanceToWalk; ++i)
        {
            if (mapX - 1 < 0 || currentLine[mapX - 1] == ' ')
            {
                if (WrapLeftToRight(currentLine, ref mapX))
                {
                    break;
                }

                continue;
            }

            if (currentLine[mapX - 1] == '#')
            {
                break;
            }

            // Take a step
            mapX--;
        }
    }

    private static bool WrapLeftToRight(ReadOnlySpan<char> currentLine, ref int mapX)
    {
        int offset = 1;

        // We look left until we find the space
        while (mapX + offset < currentLine.Length && currentLine[mapX + offset] != ' ')
        {
            offset++;
        }

        // This is the square to look at.
        offset--;
        if (currentLine[mapX + offset] == '#')
        {
            return true;
        }

        mapX += offset;
        return false;
    }

    private static void WalkDown(ReadOnlySpan<string> map, ref int mapX, ref int mapY, int distanceToWalk)
    {
        for (int i = 0; i < distanceToWalk; ++i)
        {
            if (mapY + 1 >= map.Length || mapX >= map[mapY + 1].Length || map[mapY + 1][mapX] == ' ')
            {
                if (WrapDownToUp(map, ref mapX, ref mapY))
                {
                    break;
                }

                continue;
            }

            if (map[mapY + 1][mapX] == '#')
            {
                break;
            }

            // Take a step
            mapY++;
        }
    }

    private static bool WrapDownToUp(ReadOnlySpan<string> map, ref int mapX, ref int mapY)
    {
        int offset = -1;

        // We look left until we find the space
        while (mapY + offset >= 0 && mapX < map[mapY + offset].Length && map[mapY + offset][mapX] != ' ')
        {
            offset--;
        }

        // This is the square to look at.
        offset++;
        if (map[mapY + offset][mapX] == '#')
        {
            return true;
        }

        mapY += offset;
        return false;
    }

    private static void WalkUp(ReadOnlySpan<string> map, ref int mapX, ref int mapY, int distanceToWalk)
    {
        for (int i = 0; i < distanceToWalk; ++i)
        {
            if (mapY - 1 < 0 || mapX >= map[mapY - 1].Length || map[mapY - 1][mapX] == ' ')
            {
                if (WrapUpToDown(map, ref mapX, ref mapY))
                {
                    break;
                }

                continue;
            }

            if (map[mapY - 1][mapX] == '#')
            {
                break;
            }

            // Take a step
            mapY--;
        }
    }

    private static bool WrapUpToDown(ReadOnlySpan<string> map, ref int mapX, ref int mapY)
    {
        int offset = 1;

        // We look left until we find the space
        while (mapY + offset < map.Length && mapX < map[mapY + offset].Length && map[mapY + offset][mapX] != ' ')
        {
            offset++;
        }

        // This is the square to look at.
        offset--;
        if (map[mapY + offset][mapX] == '#')
        {
            return true;
        }

        mapY += offset;
        return false;
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

    private static Direction NextDirectionClockwise(Direction direction)
    {
        int next = (int)direction + 1;
        if (next == 4)
        {
            next = 0;
        }

        return (Direction)next;
    }

    private static Direction NextDirectionAnticlockwise(Direction direction)
    {
        int next = (int)direction - 1;
        if (next == -1)
        {
            next = 3;
        }

        return (Direction)next;
    }
}
