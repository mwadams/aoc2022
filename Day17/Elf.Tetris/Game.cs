namespace Elf.Tetris
{
    using System;
    using System.Collections.Generic;

    public static class Game
    {
        public static long Process(ReadOnlySpan<char> pufferCode, long totalBlocks, bool useStats)
        {
            long blocksToDrop = totalBlocks;

            Span<byte> playArea = stackalloc byte[4500];
            Board board = new(playArea, new Puffer(pufferCode));

            Dictionary<State, (int, int)>? state = useStats ? new() : null;
            bool calculatingStats = useStats;
            int dropped = 0;

            long calculatedFromStats = 0;

            while (blocksToDrop > 0)
            {
                Blocks.Shape shape = (Blocks.Shape)((totalBlocks - blocksToDrop) % 5);
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
                        foreach(var s in state)
                        {
                            if (s.Value.Item2 == blocksToDrop + heightDropped.Dropped)
                            {
                                return calculatedFromStats += s.Value.Item1 + heightDelta;
                            }
                        }
                    }

                    state.Add(currentState, (board.StackHeight, dropped));
                }
            }

            return board.StackHeight + calculatedFromStats;
        }

    }
}
