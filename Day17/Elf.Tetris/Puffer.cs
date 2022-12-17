namespace Elf.Tetris
{
    public ref struct Puffer
    {
        private readonly ReadOnlySpan<char> directionBuffer;
        private int bufferIndex = 0;

        public Puffer(ReadOnlySpan<char> directionBuffer)
        {
            this.directionBuffer = directionBuffer;
        }

        public int NextDirection()
        {
            int result = directionBuffer[bufferIndex++] == '<' ? -1 : 1;
            if (this.bufferIndex == directionBuffer.Length)
            {
                this.bufferIndex = 0;
            }

            return result;
        }

        public int CurrentIndex => this.bufferIndex;
    }
}
