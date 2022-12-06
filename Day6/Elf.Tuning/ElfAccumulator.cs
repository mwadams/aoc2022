namespace Elf.Tuning;

using System;

internal ref struct ElfAccumulator
{
    private readonly int requiredSequenceLength;

    public ElfAccumulator(int requiredSequenceLength)
    {
        this.CharactersRead = 0;
        this.requiredSequenceLength = requiredSequenceLength;
    }

    public int CharactersRead { get; private set; }

    public void ProcessLine(string line)
    {
        this.CharactersRead = FindDistincSequence(line.AsSpan(), this.requiredSequenceLength);
    }

    private int FindDistincSequence(ReadOnlySpan<char> line, int requiredSequenceLength)
    {
        int index = 0;
        while (index <= line.Length - requiredSequenceLength)
        {
            for (int k = 0; k < requiredSequenceLength - 1; ++k) // The number of characters in sequence
            {
                for (int j = index + k + 1; j < index + requiredSequenceLength; ++j)
                {
                    if (line[index + k] == line[j])
                    {
                        index = index + k + 1; // skip ahead to the first of the duplicate pair
                        k = -1; // reset k
                        break;
                    }
                }
            }

            return index + requiredSequenceLength;
        }

        return -1;
    }
}