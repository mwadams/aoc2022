namespace Elf.SupplyStacks
{
    using System;

    /// <summary>
    /// This is a stack that supports pushing and popping substacks
    /// and shifting values into the bottom of the stack for intitialization.
    /// </summary>
    public readonly struct CrateStack
    {
        private readonly List<char> stack;

        public CrateStack()
        {
            this.stack = new();
        }

        public int Count => this.stack.Count;

        /// <summary>
        /// Shifts a value into the bottom of the stack
        /// </summary>
        /// <param name="value">The value to insert.</param>
        public void Shift(char value)
        {
            this.stack.Insert(0, value);
        }

        public char Peek()
        {
            return this.stack[^1];
        }

        public void Push(char value)
        {
            this.stack.Add(value);
        }

        public void Push(ReadOnlySpan<char> value)
        {
            foreach(char item in value)
            {
                this.stack.Add(item);
            }
        }

        /// <summary>
        /// Pops an item from the stack
        /// </summary>
        /// <returns>The item that was at the top of the stack</returns>
        public char Pop()
        {
            var result = this.stack[^1];
            this.stack.RemoveAt(this.stack.Count - 1);
            return result;
        }

        public ReadOnlySpan<char> Pop(int numberToPop)
        {
            // We need to make a copy of the result
            int index = this.stack.Count - numberToPop;
            var span = this.stack.ToArray().AsSpan().Slice(index, numberToPop);
            // And clear it from the original stack
            this.stack.RemoveRange(index, numberToPop);
            return span;
        }
    }
}