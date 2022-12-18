namespace Elf.Obsidian;
public readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        int coordinateCount = 0;
        int maxDimension = 0;
        Span<int> coordinates = stackalloc int[3 * lines.Length];

        foreach (var line in lines)
        {
            Span<int> localCoordinates = coordinates.Slice(coordinateCount, 3);
            ProcessLine(line.AsSpan(), localCoordinates);
            maxDimension = Math.Max(localCoordinates[0], Math.Max(localCoordinates[1], Math.Max(localCoordinates[2], maxDimension)));
            coordinateCount += 3;
        }

        maxDimension += 2;
        int maxDimensionSquared = maxDimension * maxDimension;

        Span<State> droplet = stackalloc State[maxDimension * maxDimension * maxDimension];

        for (int i = 0; i < coordinates.Length; i += 3)
        {
            droplet[coordinates[i] + (coordinates[i + 1] * maxDimension) + (coordinates[i + 2] * maxDimensionSquared)] = State.Exists;
        }

        FloodFill(droplet, maxDimension, maxDimensionSquared);

        long result = 0;

        for (int i = 0; i < coordinates.Length; i += 3)
        {
            int x = coordinates[i];
            int y = coordinates[i + 1];
            int z = coordinates[i + 2];

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

        return result;
    }

    private static void FloodFill(Span<State> droplet, int maxDimension, int maxDimensionSquared)
    {
        for (int x = 1; x < maxDimension - 1; x++)
        {
            for (int y = 1; y < maxDimension - 1; y++)
            {
                for (int z = 1; z < maxDimension - 1; z++)
                {
                    ref State facet = ref droplet[x + (y * maxDimension) + (z * maxDimensionSquared)];

                    if (facet == State.ConnectedToPerimeter)
                    {
                        // Tell everything it is in a bubble
                        facet = State.Bubble;
                    }
                }
            }
        }

        bool changed = true;
        while (changed)
        {
            changed = false;
            for (int x = 1; x < maxDimension - 1; ++x)
            {
                for (int y = 1; y < maxDimension - 1; ++y)
                {
                    for (int z = 1; z < maxDimension - 1; ++z)
                    {
                        ref State facet = ref droplet[x + (y * maxDimension) + (z * maxDimensionSquared)];
                        if (facet == State.Bubble &&
                            (droplet[x - 1 + (y * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter ||
                            droplet[x + 1 + (y * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter ||
                            droplet[x + ((y - 1) * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter ||
                            droplet[x + ((y + 1) * maxDimension) + (z * maxDimensionSquared)] == State.ConnectedToPerimeter ||
                            droplet[x + (y * maxDimension) + ((z - 1) * maxDimensionSquared)] == State.ConnectedToPerimeter ||
                            droplet[x + (y * maxDimension) + ((z + 1) * maxDimensionSquared)] == State.ConnectedToPerimeter))
                        {
                            changed = true;
                            facet = State.ConnectedToPerimeter;
                        }
                    }
                }
            }
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