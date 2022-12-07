namespace Elf.Space;

using System;

internal ref struct ElfAccumulator
{
    private readonly Stack<Directory> directoryStack;
    private readonly List<int> totalSizes;
    private readonly int maxSize;

    public ElfAccumulator(int maxSize)
    {
        this.maxSize = maxSize;
        this.directoryStack = new();
        this.totalSizes = new();
    }

    public int TotalAtMostMaxSize { get; private set; }

    public int TotalSize { get; private set; }

    public void ProcessLine(string line)
    {
        ReadOnlySpan<char> lineSpan = line;
        if (lineSpan[0] == '$')
        {
            this.ProcessCommand(lineSpan);
        }
        else if (lineSpan[0] >= '0' && lineSpan[0] <= '9')
        {
            this.ProcessFile(lineSpan);
        }
    }

    public void Finish()
    {
        while(this.directoryStack.Count > 0)
        {
            this.LeaveChildDirectory();
        }
    }

    public int GetDirectorySizeToFreeUp(int totalRequiredSpace, int diskSize)
    {
        int remainingSpace = diskSize - this.TotalSize;
        int remainingRequired = totalRequiredSpace - remainingSpace;
        int minValue = int.MaxValue;
        foreach(var item in this.totalSizes)
        {
            if (item >= remainingRequired && item < minValue)
            {
                minValue = item;
            }
        }

        return minValue;
    }

    private void ProcessCommand(ReadOnlySpan<char> lineSpan)
    {
        ReadOnlySpan<char> commandSpan = lineSpan[2..];

        if (commandSpan.StartsWith("cd .."))
        {
            this.LeaveChildDirectory();
        }
        else if (commandSpan.StartsWith("cd "))
        {
            this.EnterChildDirectory();
        }
    }

    private void LeaveChildDirectory()
    {
        Directory currentDirectory = this.directoryStack.Pop();
        if (currentDirectory.TotalSize <= this.maxSize)
        {
            this.TotalAtMostMaxSize += currentDirectory.TotalSize;
        }

        // Remember this directory size
        this.totalSizes.Add(currentDirectory.TotalSize);

        if (this.directoryStack.Count == 0)
        {
            this.TotalSize = currentDirectory.TotalSize;
        }
    }

    private void ProcessFile(ReadOnlySpan<char> lineSpan)
    {
        Directory currentDirectory = this.directoryStack.Peek();
        int indexOfSpan = lineSpan.IndexOf(' ');
        int size = int.Parse(lineSpan[..indexOfSpan]);
        currentDirectory.AddFile(size);

    }

    private readonly void EnterChildDirectory()
    {
        Directory newChild;
        if (this.directoryStack.TryPeek(out Directory? result))
        {
            newChild = result.AddChild();
        }
        else
        {
            newChild = new();
        }

        this.directoryStack.Push(newChild);
    }
}
