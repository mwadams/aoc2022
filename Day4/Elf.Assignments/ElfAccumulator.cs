namespace Elf.Assignments;

using System;

internal ref struct ElfAccumulator
{
    private int lineNumber;

    public ElfAccumulator()
    {
        this.OverlapCount = 0;
    }

    public int OverlapCount { get; private set; }

    public void ProcessLine(string line)
    {
        this.lineNumber++;
        if (this.IsFullyContainedOverlap(line))
        {
            this.OverlapCount++;
        }
    }

    private bool IsFullyContainedOverlap(string line)
    {
        ReadOnlySpan<char> lineSpan = line.AsSpan();
        // There must be at least 1 digit, so we don't need to look in the first char
        int firstSeparator = lineSpan[1..].IndexOf('-') + 1;
        // There must be at least 1 digit, so we don't need to look in the first char after the separator
        int rangeSeparator = lineSpan[(firstSeparator + 2)..].IndexOf(',') + (firstSeparator + 2);
        // There must be at least 1 digit, so we don't need to look in the first char after the separator
        int secondSeparator = lineSpan[(rangeSeparator + 1)..].IndexOf('-') + (rangeSeparator + 1);

        int start1 = int.Parse(lineSpan[0..firstSeparator]);
        int end1 = int.Parse(lineSpan[(firstSeparator + 1)..rangeSeparator]);
        int start2 = int.Parse(lineSpan[(rangeSeparator + 1)..secondSeparator]);
        int end2 = int.Parse(lineSpan[(secondSeparator + 1)..]);

        return
            (start1 <= start2 && end1 >= end2) ||
            (start2 <= start1 && end2 >= end1);
    }
}