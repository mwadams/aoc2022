namespace Elf.Obsidian;

using System.Collections.Generic;

internal readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        int maxDimension = 23;
        int maxDimensionSquared = maxDimension * maxDimension;

        Span<State> droplet = new State[maxDimension * maxDimension * maxDimension];

        Span<int> coordinates = stackalloc int[3];

        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan(), coordinates);

            droplet[coordinates[0] + (coordinates[1] * maxDimension) + (coordinates[2] * maxDimensionSquared)] |= State.Exists;

            ElimateAdjacentFaces(coordinates, State.Face1, maxDimension, droplet, -1, 0, 0);
            ElimateAdjacentFaces(coordinates, State.Face2, maxDimension, droplet, 1, 0, 0);
            ElimateAdjacentFaces(coordinates, State.Face3, maxDimension, droplet, 0, 1, 0);
            ElimateAdjacentFaces(coordinates, State.Face4, maxDimension, droplet, 0, -1, 0);
            ElimateAdjacentFaces(coordinates, State.Face5, maxDimension, droplet, 0, 0, 1);
            ElimateAdjacentFaces(coordinates, State.Face6, maxDimension, droplet, 0, 0, -1);
        }

        FloodFill(droplet, maxDimension);

        long result = 0;


        for (int x = 0; x < maxDimension; x++)
        {
            for (int y = 0; y < maxDimension; y++)
            {
                for (int z = 0; z < maxDimension; z++)
                {
                    var facet = droplet[x + (y * maxDimension) + (z * maxDimensionSquared)];

                    if ((facet & State.Exists) == 0)
                    {
                        continue;
                    }

                    var faceState = facet & State.Faces;

                    EliminateFacesAdjacentToAirPocket(ref faceState, x, y, z, droplet, maxDimension);

                    result += 6 - CountBits(faceState);
                }
            }
        }

        return result;
    }


    private static void FloodFill(Span<State> droplet, int maxDimension)
    {
        Stack<(int X, int Y, int Z)> pixels = new Stack<(int, int, int)>();
        Span<bool> seen = stackalloc bool[maxDimension * maxDimension * maxDimension];

        pixels.Push((0, 0, 0));

        while (pixels.Count > 0)
        {
            (int X, int Y, int Z) a = pixels.Pop();
            seen[a.X + (a.Y * maxDimension) + (a.Z * maxDimension * maxDimension)] = true;

            ref State state = ref droplet[a.X + (a.Y * maxDimension) + (a.Z * maxDimension * maxDimension)];

            if ((state & State.Exists) == 0)
            {
                state |= State.ConnectsToEdge;
                if ((a.X - 1 >= 0) && !seen[a.X - 1 + (a.Y * maxDimension) + (a.Z * maxDimension * maxDimension)])
                {
                    pixels.Push(new(a.X - 1, a.Y, a.Z));
                }

                if ((a.X + 1 < maxDimension) && !seen[a.X + 1 + (a.Y * maxDimension) + (a.Z * maxDimension * maxDimension)])
                {
                    pixels.Push(new(a.X + 1, a.Y, a.Z));
                }

                if ((a.Y - 1 >= 0) && !seen[a.X + ((a.Y - 1) * maxDimension) + (a.Z * maxDimension * maxDimension)])
                {
                    pixels.Push(new(a.X, a.Y - 1, a.Z));
                }

                if ((a.Y + 1 < maxDimension) && !seen[a.X + ((a.Y + 1) * maxDimension) + (a.Z * maxDimension * maxDimension)])
                {
                    pixels.Push(new(a.X, a.Y + 1, a.Z));
                }

                if ((a.Z - 1 >= 0) && !seen[a.X + (a.Y * maxDimension) + ((a.Z - 1) * maxDimension * maxDimension)])
                {
                    pixels.Push(new(a.X, a.Y, a.Z - 1));
                }

                if ((a.Z + 1 < maxDimension) && !seen[a.X + (a.Y * maxDimension) + ((a.Z + 1) * maxDimension * maxDimension)])
                {
                    pixels.Push(new(a.X, a.Y, a.Z + 1));
                }
            }
        }
    }

    private static void EliminateFacesAdjacentToAirPocket(ref State faceState, int x, int y, int z, Span<State> droplet, int maxDimension)
    {
        EliminateIfNoPathToOutside(ref faceState, x - 1, y, z, State.Face1, droplet, maxDimension);
        EliminateIfNoPathToOutside(ref faceState, x + 1, y, z, State.Face2, droplet, maxDimension);
        EliminateIfNoPathToOutside(ref faceState, x, y - 1, z, State.Face3, droplet, maxDimension);
        EliminateIfNoPathToOutside(ref faceState, x, y + 1, z, State.Face4, droplet, maxDimension);
        EliminateIfNoPathToOutside(ref faceState, x, y, z - 1, State.Face5, droplet, maxDimension);
        EliminateIfNoPathToOutside(ref faceState, x, y, z + 1, State.Face6, droplet, maxDimension);
    }

    private static void EliminateIfNoPathToOutside(ref State faceState, int x, int y, int z, State face, Span<State> droplet, int maxDimension)
    {
        if (x >= 0 && x < maxDimension && y >= 0 && y < maxDimension && z >= 0 && z < maxDimension &&
            (droplet[x + (y * maxDimension) + (z * maxDimension * maxDimension)] & (State.Exists | State.ConnectsToEdge)) == 0)
        {
            faceState |= face;
        }
    }

    private static int CountBits(State faceState)
    {
        return
            (((faceState & State.Face1) == 0) ? 0 : 1) +
            (((faceState & State.Face2) == 0) ? 0 : 1) +
            (((faceState & State.Face3) == 0) ? 0 : 1) +
            (((faceState & State.Face4) == 0) ? 0 : 1) +
            (((faceState & State.Face5) == 0) ? 0 : 1) +
            (((faceState & State.Face6) == 0) ? 0 : 1);
    }

    private static void ElimateAdjacentFaces(ReadOnlySpan<int> coordinates, State state, int maxDimension, Span<State> droplet, int dx, int dy, int dz)
    {
        int newX = coordinates[0] + dx;
        int newY = coordinates[1] + dy;
        int newZ = coordinates[2] + dz;

        if (newX >= 0 && newX < maxDimension && newY >= 0 && newY < maxDimension && newZ >= 0 && newZ < maxDimension)
        {
            droplet[newX + (newY * maxDimension) + (newZ * maxDimension * maxDimension)] |= state;
        }
    }

    private static void ProcessLine(ReadOnlySpan<char> line, Span<int> coordinates)
    {
        int dimensionIndex = 0;
        int lastStart = 0;
        for (int i = 0; i < line.Length; ++i)
        {
            if (line[i] == ',')
            {
                coordinates[dimensionIndex++] = int.Parse(line[lastStart..i]) + 1;
                lastStart = i + 1;
            }
        }

        // Offset the coordinates by 1 to guarantee a perimeter
        coordinates[dimensionIndex] = int.Parse(line[lastStart..]) + 1;
    }
}