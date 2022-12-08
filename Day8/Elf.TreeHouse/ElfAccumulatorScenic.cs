namespace Elf.TreeHouse;

using System;

internal ref struct ElfAccumulatorScenic
{
    private readonly string[] lines;
    private readonly LocationMetric[,] metrics;
    private readonly int width;
    private readonly int height;

    public ElfAccumulatorScenic(string[] lines)
    {
        this.lines = lines;
        this.width = lines[0].Length;
        this.height = lines.Length;
        this.metrics = new LocationMetric[this.width, this.height];
    }

    public int Result { get; private set; }

    public void BuildMetrics()
    {
        this.BuildNorthAndWest();
        this.BuildEastAndSouth();
    }

    private void BuildNorthAndWest()
    {
        for (int y = 0; y < this.height; ++y)
        {
            string line = this.lines[y];
            for (int x = 0; x < this.width; ++x)
            {
                int value = line[x] - '0';
                metrics[x, y] = new(value, GetNorthWestScenic(x, y, value));
            }
        }
    }

    private void BuildEastAndSouth()
    {
        for (int y = this.height - 1; y >= 0; --y)
        {
            for (int x = this.width - 1; x >= 0; --x)
            {
                int value = this.metrics[x, y].Value;
                metrics[x, y] = new(value, GetSouthEastScenic(x, y, value) * metrics[x, y].ScenicScore);
                BuildResult(x, y);
            }
        }
    }

    private void BuildResult(int x, int y)
    {
        this.Result = Math.Max(this.Result, this.metrics[x, y].ScenicScore);
    }

    private int GetSouthEastScenic(int x, int y, int value)
    {
        if (x == this.width - 1)
        {
            return 0;
        }

        if (y == this.height - 1)
        {
            return 0;
        }

        int xCount = 1;
        for (int x1 = x + 1; x1 < this.width - 1; ++x1)
        {
            if (this.metrics[x1, y].Value >= value)
            {
                // We have to stop here
                break;
            }

            xCount++;
        }

        int yCount = 1;
        for (int y1 = y + 1; y1 < this.height - 1; ++y1)
        {
            if (this.metrics[x, y1].Value >= value)
            {
                // We have to stop here
                break;
            }

            yCount++;
        }

        return xCount * yCount;
    }

    private int GetNorthWestScenic(int x, int y, int value)
    {
        if (x == 0)
        {
            return 0;
        }

        if (y == 0)
        {
            return 0;
        }

        int xCount = 1;
        for (int x1 = x - 1; x1 >= 1; --x1)
        {
            if (this.metrics[x1, y].Value >= value)
            {
                // We have to stop here
                break;
            }

            xCount++;
        }

        int yCount = 1;
        for (int y1 = y - 1; y1 >= 1; --y1)
        {
            if (this.metrics[x, y1].Value >= value)
            {
                // We have to stop here
                break;
            }

            yCount++;
        }

        return xCount * yCount;
    }

    internal record struct LocationMetric(int Value, int ScenicScore);
}

