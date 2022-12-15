namespace Elf.Hoopla;

public readonly record struct Range(int Low, int High) : IComparable<Range>
{
    public int CompareTo(Range other)
    {
        if (this.Low < other.Low)
        {
            return -1;
        }
        else if (this.Low == other.Low)
        {
            if (this.High < other.High)
            {
                return -1;
            }
            else if (this.High == other.High)
            {
                return 0;
            }

            return 1;
        }
        else
        {
            return 1;
        }
    }
}
