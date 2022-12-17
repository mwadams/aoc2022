namespace Elf.Tetris
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static Elf.Tetris.Blocks;

    public static class Game
    {
        public static long Process(ReadOnlySpan<char> pufferCode, long totalBlocks)
        {
            long blocksToDrop = totalBlocks;

            Span<byte> playArea = stackalloc byte[4500];
            Board board = new(playArea, new Puffer(pufferCode));

            Dictionary<State, (int, int)> state = new();
            bool calculatingStats = true;
            int dropped = 0;

            long calculatedFromStats = 0;

            while (blocksToDrop > 0)
            {
                Shape shape = (Blocks.Shape)((totalBlocks - blocksToDrop) % 5);
                board.DropShape(2, shape);
                blocksToDrop--;
                dropped++;

                if (calculatingStats)
                {
                    State currentState = board.GetState(shape);

                    if (state.TryGetValue(currentState, out (int Height, int Dropped) heightDropped))
                    {
                        long remainingItems = totalBlocks - dropped;
                        long dropDelta = dropped - heightDropped.Dropped;
                        long repeats = remainingItems / dropDelta;
                        long heightDelta = board.StackHeight - heightDropped.Height;

                        calculatedFromStats = repeats * heightDelta;
                        blocksToDrop = (int)(remainingItems % dropDelta);
                        calculatingStats = false;
                        state.Clear();
                    }

                    state.Add(currentState, (board.StackHeight, dropped));
                }
            }

            return board.StackHeight + calculatedFromStats;
        }

    }
}
