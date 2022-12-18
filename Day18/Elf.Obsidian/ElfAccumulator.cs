namespace Elf.Obsidian;

internal readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        int maxDimension = 25;
        int maxDimensionSquared = 25 * 25;

        Span<State> droplet = new State[maxDimension * maxDimension * maxDimension];

        Span<int> coordinates = stackalloc int[3];

        foreach (var line in lines)
        {
            ProcessLine(line.AsSpan(), coordinates);

            droplet[coordinates[0] + (coordinates[1] * maxDimension) + (coordinates[2] * maxDimensionSquared)] |= State.Exists;

            SetFaceAdjacency(coordinates, State.Face1, maxDimension, droplet, -1, 0, 0);
            SetFaceAdjacency(coordinates, State.Face2, maxDimension, droplet, 1, 0, 0);
            SetFaceAdjacency(coordinates, State.Face3, maxDimension, droplet, 0, 1, 0);
            SetFaceAdjacency(coordinates, State.Face4, maxDimension, droplet, 0, -1, 0);
            SetFaceAdjacency(coordinates, State.Face5, maxDimension, droplet, 0, 0, 1);
            SetFaceAdjacency(coordinates, State.Face6, maxDimension, droplet, 0, 0, -1);
        }

        long result = 0;
        int x = 0;
        int y = 0;
        int z = 0;


        foreach (var facet in droplet)
        {

            if ((facet & State.Exists) == 0)
            {
                continue;
            }

            var faceState = facet & State.Faces;

            EliminateFaces(ref faceState, x, y, z, droplet, maxDimension);

            result += 6 - CountBits(faceState);

            x += 1;
            if (x > maxDimension)
            {
                x = 0;
                y += 1;
                if (y > maxDimension)
                {
                    y = 0;
                    z += 1;
                }
            }
        }

        return result;
    }

    private void EliminateFaces(ref State faceState, int x, int y, int z, Span<State> droplet, int maxDimension)
    {
        // Run a search for a path to th
    }

    private int CountBits(State faceState)
    {
        return
            (((faceState & State.Face1) == 0) ? 0 : 1) +
            (((faceState & State.Face2) == 0) ? 0 : 1) +
            (((faceState & State.Face3) == 0) ? 0 : 1) +
            (((faceState & State.Face4) == 0) ? 0 : 1) +
            (((faceState & State.Face5) == 0) ? 0 : 1) +
            (((faceState & State.Face6) == 0) ? 0 : 1);
    }

    private static void SetFaceAdjacency(ReadOnlySpan<int> coordinates, State state, int maxDimension, Span<State> droplet, int dx, int dy, int dz)
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
                coordinates[dimensionIndex++] = int.Parse(line[lastStart..i]);
                lastStart = i + 1;
            }
        }

        coordinates[dimensionIndex] = int.Parse(line[lastStart..]);
    }
}