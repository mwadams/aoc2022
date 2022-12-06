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
        // Initial check for line length mismatch
        if (line.Length < requiredSequenceLength)
        {
            return -1;
        }

        int startOfSequenceIndex = 0;
        int indexOfAnchorInSequence = 0;
        // startOfSequenceIndex marks the start of a group of requiredSequenceLength characters
        // (startOfSequenceIndex + indexOfAnchorInSequence) is the anchor character within that group that we are comparing to each subsequent character
        // on this iteration.
        // We only need to iterate through (requiredSequenceLength - 1) as our anchor in the group, because we don't need to 
        // compare the last character to itself!

        while (indexOfAnchorInSequence < requiredSequenceLength - 1)
        {
            // This is the look-ahead loop to compare the anchor character at (startOfSequenceIndex + indexOfAnchorInSequence) with each subsequent character in the group;
            // (this inner loop naturally decreases in length by 1 each time we move the anchor character in the group up by 1) 
            for (int lookAheadIndexInGroup = startOfSequenceIndex + indexOfAnchorInSequence + 1; lookAheadIndexInGroup < startOfSequenceIndex + requiredSequenceLength; ++lookAheadIndexInGroup)
            {
                // Look for a duplicate pair
                if (line[startOfSequenceIndex + indexOfAnchorInSequence] == line[lookAheadIndexInGroup])
                {
                    // skip ahead past the first of the duplicate pair
                    startOfSequenceIndex += indexOfAnchorInSequence + 1;

                    // Check we haven't run out of line
                    if (startOfSequenceIndex > line.Length - requiredSequenceLength)
                    {
                        return -1;
                    }

                    // reset k to start from the beginning of the new group (it will be incremented by the outer loop to start a 0)
                    indexOfAnchorInSequence = -1;

                    break;
                }
            }

            // Move to the next anchor in the group
            ++indexOfAnchorInSequence;
        }

        // If we got to the end of the group, we have found a non-duplicate set.
        return startOfSequenceIndex + requiredSequenceLength;
    }
}