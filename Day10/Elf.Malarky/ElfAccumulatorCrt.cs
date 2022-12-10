////namespace Elf.Malarky;

////internal ref struct ElfAccumulatorCrt
////{
////    private readonly int initialInterval;
////    private readonly int sampleInterval;
////    private char[,] screen;

////    private int cycle = 0;
////    private int beamPositionX = 0;
////    private int beamPositionY = 0;
////    private int xRegister = 1;

////    public ElfAccumulatorCrt(int initialInterval, int sampleInterval)
////    {
////        this.initialInterval = initialInterval;
////        this.sampleInterval = sampleInterval;
////        this.AccumulatedSignalStrength = 0;
////    }

////    public int AccumulatedSignalStrength { get; private set; }

////    public void ProcessLine(string line)
////    {
////        ReadOnlySpan<char> lineSpan = line.AsSpan();
////        switch (lineSpan[0])
////        {
////            case 'a':
////                ProcessAddX(int.Parse(lineSpan[5..]));
////                break;
////            case 'n':
////                ProcessNop();
////                break;
////            default:
////                throw new InvalidOperationException("Unrecognized instruction.");

////        }
////    }

////    private void ProcessNop()
////    {
////        this.HandleCycle();
////    }

////    private void ProcessAddX(int v)
////    {
////        this.HandleCycle();
////        this.HandleCycle();
////        this.xRegister += v;
////    }

////    private void HandleCycle()
////    {
////        this.cycle++;
////        HandleCrt();

////        if ((this.cycle - this.initialInterval) % this.sampleInterval == 0)
////        {
////            this.AccumulatedSignalStrength += this.xRegister * this.cycle;
////        }
////    }

////    private void HandleCrt()
////    {
////        if (this.beamPositionX == this.crtWidth - 1)
////        {
////            this.beamPositionX = 0;
////            if (this.beamPositionY == this.crtHeight - 1)
////            {
////                this.beamPositionY = 0;
////            }
////            else
////            {
////                this.beamPositionY++;
////            }
////        }
////        else
////        {
////            this.beamPositionY++;
////        }

////        this.HandleBeamCollision();
////    }

////    private void HandleBeamCollision()
////    {
////        if (this.beamPositionY == 0 &&
////            this.beamPositionX <= this.xRegister &&
////            this.beamPositionX >= this.xRegister -1)
////        {
////            Console.
////        }
////    }
////}