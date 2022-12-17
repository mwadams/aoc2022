namespace Elf.Tetris
{
    public static class Blocks
    {
        public enum Shape : byte
        {
            Horz4,
            Plus,
            BackL,
            Vert4,
            Square,
        }

        public static readonly byte[][] Horz4 = new byte[][] {
            new byte[] { 0b11110000, 0b01111000, 0b00111100, 0b00011110 }
        };

        public static readonly byte[][] Plus = new byte[][] {
            new byte[] { 0b01000000, 0b00100000, 0b00010000, 0b00001000, 0b00000100 },
            new byte[] { 0b11100000, 0b01110000, 0b00111000, 0b00011100, 0b00001110 },
            new byte[] { 0b01000000, 0b00100000, 0b00010000, 0b00001000, 0b00000100 }
        };

        public static readonly byte[][] BackL = new byte[][] {
            new byte[] { 0b00100000, 0b00010000, 0b00001000, 0b00000100, 0b00000010 },
            new byte[] { 0b00100000, 0b00010000, 0b00001000, 0b00000100, 0b00000010 },
            new byte[] { 0b11100000, 0b01110000, 0b00111000, 0b00011100, 0b00001110 }
        };

        public static readonly byte[][] Vert4 = new byte[][] {
            new byte[] { 0b10000000, 0b01000000, 0b00100000, 0b00010000, 0b00001000, 0b00000100, 0b00000010 },
            new byte[] { 0b10000000, 0b01000000, 0b00100000, 0b00010000, 0b00001000, 0b00000100, 0b00000010 },
            new byte[] { 0b10000000, 0b01000000, 0b00100000, 0b00010000, 0b00001000, 0b00000100, 0b00000010 },
            new byte[] { 0b10000000, 0b01000000, 0b00100000, 0b00010000, 0b00001000, 0b00000100, 0b00000010 }
        };

        public static readonly byte[][] Square = new byte[][] {
            new byte[] { 0b11000000, 0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00000110 },
            new byte[] { 0b11000000, 0b01100000, 0b00110000, 0b00011000, 0b00001100, 0b00000110 },
        };

        public static Span<byte[]> GetShape(Shape shape)
        {
            return shape switch
            {
                Shape.Horz4 => Horz4.AsSpan(),
                Shape.Plus => Plus.AsSpan(),
                Shape.BackL => BackL.AsSpan(),
                Shape.Vert4 => Vert4.AsSpan(),
                Shape.Square => Square.AsSpan(),
                _ => throw new NotSupportedException()
            };
        }
    }
}
