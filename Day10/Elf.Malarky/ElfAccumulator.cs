namespace Elf.Malarky;

internal ref struct ElfAccumulator
{
    private readonly int initialInterval;
    private readonly int sampleInterval;

    private int cycle = 0;
    private int xRegister = 1;

    public ElfAccumulator(int initialInterval, int sampleInterval)
    {
        this.initialInterval = initialInterval;
        this.sampleInterval = sampleInterval;
        this.AccumulatedSignalStrength = 0;
    }

    public int AccumulatedSignalStrength { get; private set; }

    public void ProcessLine(string line)
    {
        ReadOnlySpan<char> lineSpan = line.AsSpan();
        switch (lineSpan[0])
        {
            case 'a':
                ProcessAddX(int.Parse(lineSpan[5..]));
                break;
            case 'n':
                ProcessNop();
                break;
            default:
                throw new InvalidOperationException("Unrecognized instruction.");

        }
    }

    private void ProcessNop()
    {
        this.HandleCycle();
    }

    private void ProcessAddX(int v)
    {
        this.HandleCycle();
        this.HandleCycle();
        this.xRegister += v;
    }

    private void HandleCycle()
    {
        this.cycle++;

        if ((this.cycle - this.initialInterval) % this.sampleInterval == 0)
        {
            this.AccumulatedSignalStrength += this.xRegister * this.cycle;
        }
    }
}