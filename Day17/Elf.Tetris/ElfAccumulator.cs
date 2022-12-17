﻿namespace Elf.Tetris;

using static global::Elf.Tetris.Blocks;

internal readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {       
        return Game.Process(lines[0].AsSpan(), 2022);
    }
}