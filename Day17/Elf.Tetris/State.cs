namespace Elf.Tetris;

public readonly record struct State(short Col1, short Col2, short Col3, short Col4, short Col5, short Col6, short Col7, Blocks.Shape Shape, int PufferIndex)
{
}