namespace Elf.Diffusion;

public readonly ref struct ElfAccumulatorB
{
    private readonly string[] lines;

    public ElfAccumulatorB(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        return ProcessGrove(this.lines.AsSpan());
    }

    private static long ProcessGrove(ReadOnlySpan<string> grove)
    {
        int groveWidth = grove[0].Length;
        int groveHeight = grove.Length;

        HashSet<ElfPosition> elves = new(groveWidth * groveHeight);
        BuildElfPositions(grove, groveWidth, groveHeight, elves);

        return DiffuseElves(elves, 10);
    }

    private static readonly Delta[] Deltas = new Delta[] { new(0, -1), new(0, 1), new(-1, 0), new(1, 0) };
    private static readonly Check[] Checks = new Check[]
    {
        new Check(
            new Delta[] {new(0,-1), new(1,-1), new(-1,-1) },
            new Delta[] {new(0,1), new(-1,0), new(-1, 1), new(1,0), new(1,1) }),
        new Check(
            new Delta[] { new(0,1), new(1,1), new(-1,1) },
            new Delta[] { new (0,-1), new (-1,0), new (-1,-1), new (1,0), new (1,-1) }),
        new Check(
            new Delta[] { new(-1,0), new(-1,-1), new(-1,1) },
            new Delta[] { new (0,-1), new (0,1), new (1,0), new (1,1), new (1,-1) }),
        new Check(
            new Delta[] { new(1,0), new(1,-1), new(1,1) },
            new Delta[] { new (0,-1), new (0,1), new (-1,0), new (-1, 1), new (-1,-1)}),
    };



    private static long DiffuseElves(HashSet<ElfPosition> elves, int numberOfRounds)
    {
        // Maximum possible number of proposals
        Dictionary<ElfPosition, Proposal> proposals = new Dictionary<ElfPosition, Proposal>(elves.Count);
        int deltaIndex = 0;

        for (int round = 0; round < numberOfRounds; ++round)
        {
            // Clear the proposals.
            proposals.Clear();

            foreach (ElfPosition elf in elves)
            {
                CheckAndAdd(elf, elves, proposals, deltaIndex);
            }

            int moved = 0;

            foreach ((ElfPosition proposedPosition, Proposal proposal) in proposals)
            {
                if (proposal.Ignore)
                {
                    continue;
                }

                if (!proposals.ContainsKey(proposal.OriginalPosition))
                {
                    elves.Remove(proposal.OriginalPosition);
                }
                if (!elves.Contains(proposedPosition))
                {
                    elves.Add(proposedPosition);
                }

                moved++;
            }

            if (moved == 0)
            {
                // We didn't need to move anyone
                break;
            }

            deltaIndex = (deltaIndex + 1) % Deltas.Length;
        }

        FindBoundingBox(elves, out long boxWidth, out long boxHeight);
        return (boxWidth * boxHeight) - elves.Count;
    }

    private static void FindBoundingBox(HashSet<ElfPosition> elves, out long boxWidth, out long boxHeight)
    {
        long minX = long.MaxValue, minY = long.MaxValue, maxX = long.MinValue, maxY = long.MinValue;
        foreach (var elf in elves)
        {
            UpdateBoundingBox(ref minX, ref minY, ref maxX, ref maxY, elf);

        }
        boxWidth = Math.Abs(maxX - minX) + 1;
        boxHeight = Math.Abs(maxY - minY) + 1;
    }

    private static void CheckAndAdd(in ElfPosition elf, HashSet<ElfPosition> elves, Dictionary<ElfPosition, Proposal> proposals, int deltaIndex)
    {
        for (int i = 0; i < Deltas.Length; ++i)
        {
            int currentIndex = (i + deltaIndex) % Deltas.Length;
            var check = Checks[currentIndex];
            if (!PassesCheck(elf, elves, check))
            {
                continue;
            }

            if (i == 0 && IsFree(elf, elves, check))
            {
                // The elf doesn't need to move
                return;
            }

            ElfPosition proposedPosition = new(elf.X + Deltas[currentIndex].X, elf.Y + Deltas[currentIndex].Y);
            if (proposals.ContainsKey(proposedPosition))
            {
                proposals[proposedPosition] = new(default, true);
            }
            else
            {
                proposals[proposedPosition] = new(elf, false);
            }

            return;
        }
    }

    private static bool PassesCheck(in ElfPosition elf, HashSet<ElfPosition> elves, in Check check)
    {
        return
            !(elves.Contains(new(elf.X + check.Required[0].X, elf.Y + check.Required[0].Y)) ||
            elves.Contains(new(elf.X + check.Required[1].X, elf.Y + check.Required[1].Y)) ||
            elves.Contains(new(elf.X + check.Required[2].X, elf.Y + check.Required[2].Y)));
    }

    private static bool IsFree(in ElfPosition elf, HashSet<ElfPosition> elves, in Check check)
    {
        return
            !(elves.Contains(new(elf.X + check.Others[0].X, elf.Y + check.Others[0].Y)) ||
            elves.Contains(new(elf.X + check.Others[1].X, elf.Y + check.Others[1].Y)) ||
            elves.Contains(new(elf.X + check.Others[2].X, elf.Y + check.Others[2].Y)) ||
            elves.Contains(new(elf.X + check.Others[3].X, elf.Y + check.Others[3].Y)) ||
            elves.Contains(new(elf.X + check.Others[4].X, elf.Y + check.Others[4].Y)));
    }

    private static void UpdateBoundingBox(ref long minX, ref long minY, ref long maxX, ref long maxY, in ElfPosition proposedPosition)
    {
        minX = Math.Min(minX, proposedPosition.X);
        maxX = Math.Max(maxX, proposedPosition.X);
        minY = Math.Min(minY, proposedPosition.Y);
        maxY = Math.Max(maxY, proposedPosition.Y);
    }

    private static void BuildElfPositions(ReadOnlySpan<string> grove, int groveWidth, int groveHeight, HashSet<ElfPosition> elves)
    {
        for (int y = 0; y < groveHeight; ++y)
        {
            for (int x = 0; x < groveWidth; ++x)
            {
                if (grove[y][x] == '#')
                {
                    elves.Add(new(x, y));
                }
            }
        }
    }

    internal readonly record struct ElfPosition(int X, int Y);
    internal readonly record struct Delta(int X, int Y);
    internal readonly record struct Check(Delta[] Required, Delta[] Others);
    internal readonly record struct Proposal(ElfPosition OriginalPosition, bool Ignore);
}

