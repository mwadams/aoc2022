namespace Elf.RuckPack;

using System;

internal ref struct ElfAccumulator
{
    private int lineNumber;

    public ElfAccumulator()
    {
        this.PrioritySum = 0;
    }

    public int PrioritySum { get; private set; }

    public void ProcessLine(string line)
    {
        this.lineNumber++;
        this.PrioritySum += this.ScoreLine(line);
    }

    private int ScoreLine(string line)
    {
        int halfLineLength = line.Length / 2;

        if (line.Length % 2 != 0)
        {
            throw new InvalidOperationException($"Line {this.lineNumber} did not have the correct number of entries.");
        }

        for (int i = 0; i < halfLineLength; ++i)
        {
            for (int j = halfLineLength; j < line.Length; ++j)
            {
                if (line[i] == line[j])
                {
                    // We can leave immediately as there is only 1 error per line
                    return ScoreChar(line[i]);
                }
            }
        }

        throw new InvalidOperationException($"Line {this.lineNumber} had no mispacked items.");
    }

    private static int ScoreChar(char v)
    {
        if (v >= 'a' && v <= 'z')
        {
            return ((int)v - (int)'a' + 1);
        }

        return ((int)v - (int)'A' + 27);
    }
}