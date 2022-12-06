namespace Elf.SupplyStacks;

using System;
using System.Text;

internal ref struct ElfAccumulator
{
    private readonly List<CrateStack> stacks;
    private readonly bool moveInPiles;

    private enum State
    {
        Loading,
        WaitingForInstructions,
        Moving,
    }

    public ElfAccumulator(bool moveInPiles)
    {
        this.CurrentState = State.Loading;
        this.stacks = new();
        this.moveInPiles = moveInPiles;
    }

    /// <summary>
    /// Gets the current state of processing.
    /// </summary>
    private State CurrentState { get; set; }

    public void ProcessLine(string line)
    {
        switch (this.CurrentState)
        {
            case State.Loading:
                HandleLoading(line);
                break;
            case State.WaitingForInstructions:
                HandlWaitingForInstructions(line);
                break;
            case State.Moving:
                HandleMoving(line);
                break;
        }
    }

    public string GetTopOfStacks()
    {
        StringBuilder builder = new();
        foreach (var stack in this.stacks)
        {
            if (stack.Count > 0)
            {
                builder.Append(stack.Peek());
            }
            else
            {
                builder.Append(" ");
            }
        }

        return builder.ToString();
    }

    private void HandleMoving(string line)
    {
        // Slice off the "move"
        ReadOnlySpan<char> lineSlice = line.AsSpan()[5..];

        int indexOfSpace1 = lineSlice.IndexOf(' ');

        int numberToMove = int.Parse(lineSlice[..indexOfSpace1]);

        int indexOfSpace2 = lineSlice[(indexOfSpace1 + 6)..].IndexOf(' ') + indexOfSpace1 + 6;

        int from = int.Parse(lineSlice[(indexOfSpace1 + 6)..indexOfSpace2]);

        int to = int.Parse(lineSlice[(indexOfSpace2 + 4)..]);

        if (moveInPiles)
        {
            MoveInPiles(numberToMove, from, to);
        }
        else
        {
            MoveIndividually(numberToMove, from, to);
        }
    }

    private readonly void MoveIndividually(int numberToMove, int from, int to)
    {
        for (int i = 0; i < numberToMove; i++)
        {
            // Compensate for the 1-based indexing
            this.stacks[to - 1].Push(this.stacks[from - 1].Pop());
        }
    }

    private readonly void MoveInPiles(int numberToMove, int from, int to)
    {
        this.stacks[to - 1].Push(this.stacks[from - 1].Pop(numberToMove));
    }

    private void HandlWaitingForInstructions(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            this.CurrentState = State.Moving;
        }
    }

    private void HandleLoading(string line)
    {
        if (line.StartsWith(" 1"))
        {
            this.CurrentState = State.WaitingForInstructions;
            return;
        }

        int index = 1;
        int stackIndex = 0;
        while (index < line.Length)
        {
            if (this.stacks.Count < (stackIndex + 1))
            {
                // Add a new cratestack if it didn't already exist
                this.stacks.Add(new());
            }

            char character = line[index];
            if (character != ' ')
            {
                this.stacks[stackIndex].Shift(character);
            }

            //Advance to the next character
            index += 4;
            // Advance to the next stack
            stackIndex++;
        }
    }
}