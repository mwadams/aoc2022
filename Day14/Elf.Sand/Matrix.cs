namespace Elf.Sand
{
    using System;
    using System.Drawing;

    internal ref struct Matrix
    {
        private readonly Span<char> buffer;
        private readonly int minX;
        private readonly int minY;
        private readonly int height;
        private readonly int width;
        private readonly int minBoundH;
        private readonly int minBoundV;
        private readonly int floor;

        public Matrix(in Point minimum, in Point maximum, Span<char> buffer)
        {
            this.buffer = buffer;
            this.buffer.Fill('.');
            this.minX = minimum.X;
            this.minY = minimum.Y;
            this.height = (maximum.Y - minimum.Y) + 1;
            this.width = (maximum.X - minimum.X) + 1;
            this.minBoundH = this.minX + this.width;
            this.minBoundV = this.minY + this.height;
            this.floor = this.minY + this.height - 1;
        }

        public readonly bool IsInBounds(in Point p)
        {
            return IsInBounds(p.X, p.Y);
        }

        public readonly bool IsInBounds(int x, int y)
        {
            return
                IsInBoundsH(x) &&
                IsInBoundsV(y);
        }

        public readonly char Get(in Point p)
        {
            return Get(p.X, p.Y);
        }

        public readonly char Get(int x, int y)
        {
            return buffer[CalculatePoint(x, y)];
        }

        private readonly int CalculatePoint(int x, int y)
        {
            return x - this.minX + ((y - this.minY) * this.width);
        }

        public void Set(in Point p, char value)
        {
            Set(p.X, p.Y, value);
        }

        public void Set(int x, int y, char value)
        {
            buffer[CalculatePoint(x, y)] = value;
        }

        public readonly void WriteToConsole(Point entry)
        {
            int entryIndex = CalculatePoint(entry.X, entry.Y);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i % width == 0)
                {
                    Console.WriteLine();
                }

                if (i == entryIndex)
                {
                    Console.Write('+');
                }
                else
                {
                    Console.Write(this.buffer[i]);
                }
            }

            Console.WriteLine();
        }

        public readonly void WriteToFile(string filename, Point entry)
        {
            int entryIndex = CalculatePoint(entry.X, entry.Y);
            using var writer = File.CreateText(filename);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (i % width == 0)
                {
                    writer.WriteLine();
                }

                if (i == entryIndex)
                {
                    writer.Write('+');
                }
                else
                {
                    writer.Write(this.buffer[i]);
                }
            }

            writer.WriteLine();
        }

        public readonly bool IsOnFloor(int y)
        {
            return y == this.floor;
        }

        public readonly bool IsInBoundsH(int p)
        {
            return p >= this.minX && p < this.minBoundH;
        }

        public readonly bool IsInBoundsV(int p)
        {
            return p >= this.minY && p < this.minBoundV;
        }
    }
}
