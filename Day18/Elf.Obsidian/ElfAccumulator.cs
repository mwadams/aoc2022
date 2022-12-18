namespace Elf.Obsidian;

using System.Collections.Generic;

internal readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        int maxDimension = 30;
        int maxDimensionSquared = maxDimension * maxDimension;

        Span<State> droplet = stackalloc State[maxDimension * maxDimension * maxDimension];

        Span<int> coordinates = stackalloc int[3];

        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan(), coordinates);

            droplet[coordinates[0] + (coordinates[1] * maxDimension) + (coordinates[2] * maxDimensionSquared)] = State.Exists;
        }

        long result = 0;


        for (int x = 1; x < maxDimension - 1; x++)
        {
            for (int y = 1; y < maxDimension - 1; y++)
            {
                for (int z = 1; z < maxDimension - 1; z++)
                {
                    var facet = droplet[x + (y * maxDimension) + (z * maxDimensionSquared)];

                    if (facet != State.Exists)
                    {
                        continue;
                    }

                    if (droplet[x - 1 + (y * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter)
                    {
                        result++;
                    }
                    if (droplet[x + 1 + (y * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter)
                    {
                        result++;
                    }
                    if (droplet[x + ((y - 1) * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter)
                    {
                        result++;
                    }
                    if (droplet[x + ((y + 1) * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter)
                    {
                        result++;
                    }
                    if (droplet[x + (y * maxDimension) + ((z - 1) * maxDimensionSquared)] == State.ConnectedToPerimeter)
                    {
                        result++;
                    }
                    if (droplet[x + (y * maxDimension) + ((z + 1) * maxDimensionSquared)] == State.ConnectedToPerimeter)
                    {
                        result++;
                    }
                }
            }
        }

        return result;
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