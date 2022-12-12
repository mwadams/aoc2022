namespace Elf.Larks;

using System.Collections.Generic;

internal ref struct ElfAccumulatorPt1
{
    private readonly string[] lines;

    public ElfAccumulatorPt1(string[] lines)
    {
        this.lines = lines;
    }
}