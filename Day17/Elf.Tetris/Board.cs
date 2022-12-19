namespace Elf.Tetris
{
    using System;

    public ref struct Board
    {
        private Puffer puffer;

        private Span<byte> playArea;
        private int stackHeight;

        public int StackHeight => this.stackHeight;

        public Board(Span<byte> playArea, in Puffer puffer)
        {
            this.playArea = playArea;
            this.puffer = puffer;
            this.stackHeight = 0;
        }

        public State GetState(Blocks.Shape shape)
        {
            bool found1 = false, found2 = false, found3 = false, found4 = false, found5 = false, found6 = false, found7 = false;
            short col1 = 0, col2 = 0, col3 = 0, col4 = 0, col5 = 0, col6 = 0, col7 = 0;
            for(int i = stackHeight - 1; i >= 0; --i)
            {
                if (found1 && found2 && found3 && found4 && found5 && found6 && found7)
                {
                    break;
                }

                byte row = this.playArea[i];

                if (!found1 && (row & 0b10000000) == 0)
                {
                    col1++;
                }
                else
                {
                    found1 = true;
                }

                if (!found2 && (row & 0b01000000) == 0)
                {
                    col2++;
                }
                else
                {
                    found2 = true;
                }

                if (!found3 && (row & 0b00100000) == 0)
                {
                    col3++;
                }
                else
                {
                    found3 = true;
                }

                if (!found4 && (row & 0b00010000) == 0)
                {
                    col4++;
                }
                else
                {
                    found4 = true;
                }

                if (!found5 && (row & 0b00001000) == 0)
                {
                    col5++;
                }
                else
                {
                    found5 = true;
                }

                if (!found6 && (row & 0b00000100) != 0b00000100)
                {
                    col6++;
                }
                else
                {
                    found6 = true;
                }

                if (!found7 && (row & 0b00000010) != 0b00000010)
                {
                    col7++;
                }
                else
                {
                    found7 = true;
                }
            }

            return new State(col1, col2, col3, col4, col5, col6, col7, shape, puffer.CurrentIndex);
        }

        public void DropShape(int leftOffset, Blocks.Shape shape)
        {
            int currentLeftOffset = leftOffset;
            int positionAboveBoard = 3;
            ReadOnlySpan<byte[]> block = Blocks.GetShape(shape);

            while (true)
            {
                this.PushBlock(block, ref currentLeftOffset, ref positionAboveBoard);
                if (!this.DropBlock(block, ref currentLeftOffset, ref positionAboveBoard))
                {
                    break;
                }
            }

            this.StopBlock(block, currentLeftOffset, positionAboveBoard);

        }

        public void DrawBoard()
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            for (int i = stackHeight - 1; i >= Math.Max(0, stackHeight - 30); i--)
            {
                byte row = this.playArea[i];

                Console.Write('|');
                for (int j = 7; j >= 0; --j)
                {
                    Console.Write((row & (1 << j)) == 0 ? (j == 0 ? '|' : '.') : '#');
                }
                Console.WriteLine();
            }
            Console.WriteLine("|-------|");
            Console.WriteLine($"Stack Height: {stackHeight}");
            Console.ReadLine();
        }

        private void StopBlock(ReadOnlySpan<byte[]> block, int leftOffset, int positionAboveBoard)
        {
            int currentY = positionAboveBoard;
            int blockIndex = block.Length - 1;
            while (blockIndex >= 0 && (stackHeight + currentY >= 0))
            {
                // Get the right block row
                byte blockRow = block[blockIndex][leftOffset];

                // Or the board row into the board row
                this.playArea[stackHeight + currentY] = (byte)(this.playArea[stackHeight + currentY] | blockRow);

                blockIndex--;
                currentY++;
            }

            // The stack height is updated to the and point of the tests
            if (positionAboveBoard + block.Length > 0)
            {
                stackHeight += positionAboveBoard + block.Length;
            }
        }

        private bool DropBlock(ReadOnlySpan<byte[]> block, ref int leftOffset, ref int positionAboveBoard)
        {
            if (positionAboveBoard + stackHeight == 0)
            {
                return false;
            }

            int testPositionAboveBoard = positionAboveBoard - 1;

            if (!TestBlock(leftOffset, testPositionAboveBoard, block))
            {
                // We've hit something in the stack, so we can't move down
                return false;
            }

            if (testPositionAboveBoard + stackHeight >= 0)
            {
                positionAboveBoard = testPositionAboveBoard;
            }

            return true;
        }

        private void PushBlock(ReadOnlySpan<byte[]> block, ref int leftOffset, ref int positionAboveBoard)
        {
            int pufferDirection = this.puffer.NextDirection();
            int testLeftOffset = leftOffset + pufferDirection;

            if (testLeftOffset < 0)
            {
                // We don't move left, but we don't stop either.
                return;
            }

            if (testLeftOffset >= block[0].Length)
            {
                // We can't move right either, but again we don't stop.
                return;
            }

            // We are now "in the stacks" so we need to work
            if (!TestBlock(testLeftOffset, positionAboveBoard, block))
            {
                return;
            }

            leftOffset = testLeftOffset;
        }

        private bool TestBlock(int leftOffset, int positionAboveBoard, ReadOnlySpan<byte[]> block)
        {
            if (positionAboveBoard > 0 || stackHeight == 0)
            {
                // No ned to test above the board, or if there is nothing in 
                return true;
            }

            int currentY = positionAboveBoard;
            int blockIndex = block.Length - 1;
            while (blockIndex >= 0 && (stackHeight + currentY >= 0))
            {
                // Get the right board row
                byte boardRow = this.playArea[stackHeight + currentY];
                // Get the right block row
                byte blockRow = block[blockIndex][leftOffset];
                if ((boardRow & blockRow) != 0)
                {
                    return false;
                }

                blockIndex--;
                currentY++;
            }

            return true;
        }
    }
}
