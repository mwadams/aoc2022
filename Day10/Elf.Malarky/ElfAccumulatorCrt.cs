namespace Elf.Malarky;

internal ref struct ElfAccumulatorCrt
{
    private readonly int crtWidth;
    private readonly int crtHeight;
    private readonly char[,] screenBuffer;

    private int beamPositionX = 0;
    private int beamPositionY = 0;
    private int xRegister = 1;

    public ElfAccumulatorCrt(int crtWidth, int crtHeight)
    {
        this.crtWidth = crtWidth;
        this.crtHeight = crtHeight;
        this.screenBuffer = new char[crtWidth, crtHeight];        
    }


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

    public void DrawScreen()
    {
        for(int y = 0; y < this.crtHeight; ++y)
        {
            Console.WriteLine();
            for(int x = 0; x < this.crtWidth; ++x)
            {
                Console.Write(screenBuffer[x, y]);
            }
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
        this.HandleSprite();
        this.MoveBeam();
    }

    private void MoveBeam()
    {
        if (this.beamPositionX == this.crtWidth - 1)
        {
            this.beamPositionX = 0;
            if (this.beamPositionY == this.crtHeight - 1)
            {
                this.beamPositionY = 0;
            }
            else
            {
                this.beamPositionY++;
            }
        }
        else
        {
            this.beamPositionX++;
        }
    }

    private void HandleSprite()
    {
        if (this.beamPositionX >= (this.xRegister - 1) &&
            this.beamPositionX <= this.xRegister + 1)
        {
            this.screenBuffer[this.beamPositionX, this.beamPositionY] = '#';
        }
        else
        {
            this.screenBuffer[this.beamPositionX, this.beamPositionY] = '.';
        }
    }
}