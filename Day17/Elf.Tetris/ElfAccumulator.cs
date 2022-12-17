namespace Elf.Tetris;

internal readonly ref struct ElfAccumulator
{
    private readonly string[] lines;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
    }

    public int Process()
    {
        int totalBlocks = 2022;
        int blocksToDrop = totalBlocks;

        Span<byte> playArea = stackalloc byte[blocksToDrop*4];
        Board board = new(playArea, new Puffer(lines[0].AsSpan()));

        while(blocksToDrop > 0)
        {
            board.DropShape(2, (Blocks.Shape)((totalBlocks - blocksToDrop) % 5));
            blocksToDrop--;
        }
        return board.StackHeight;
    }
}