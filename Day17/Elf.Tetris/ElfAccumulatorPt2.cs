namespace Elf.Tetris;


public readonly ref struct ElfAccumulatorPt2
{
    private readonly string[] lines;

    public ElfAccumulatorPt2(string[] lines)
    {
        this.lines = lines;
    }

    public long Process()
    {
        return Game.Process(lines[0].AsSpan(), 1000000000000);
    }
}