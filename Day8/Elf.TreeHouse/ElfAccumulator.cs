namespace Elf.TreeHouse;

using System;

internal ref struct ElfAccumulator
{
    private readonly string[] lines;
    private readonly LocationMetric[,] metrics;
    private readonly int width;
    private readonly int height;

    public ElfAccumulator(string[] lines)
    {
        this.lines = lines;
        this.width = lines[0].Length;
        this.height = lines.Length;
        this.metrics = new LocationMetric[this.width, this.height];
    }

    public int VisibleCount { get; private set; }

    public int MaxScenicScore { get; private set; }

    public void BuildMetrics()
    {
        this.BuildNorthAndWest();
        this.BuildEastAndSouth();
        this.CountResults();
    }

    private void CountResults()
    {
        for (int y = 0; y < this.height; ++y)
        {
            for (int x = 0; x < this.width; ++x)
            {
                if (this.IsVisible(x, y))
                {
                    this.VisibleCount++;
                }

                this.MaxScenicScore = Math.Max(this.MaxScenicScore, this.metrics[x, y].ScenicScore);
            }
        }
    }

    private void BuildNorthAndWest()
    {
        for (int y = 0; y < this.height; ++y)
        {
            string line = this.lines[y];
            for (int x = 0; x < this.width; ++x)
            {
                int value = line[x] - '0';
                metrics[x, y] = new(value, GetMaxNorth(x, y, value), metrics[x, y].MaxEast, metrics[x, y].MaxSouth, GetMaxWest(x, y, value), GetNorthWestScenic(x, y, value));
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
                metrics[x, y] = new(value, metrics[x, y].MaxNorth, GetMaxEast(x, y, value), GetMaxSouth(x, y, value), metrics[x, y].MaxWest, GetSouthEastScenic(x, y, value) * metrics[x, y].ScenicScore);
            }
        }
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

    private bool IsVisible(int x, int y)
    {
        int value = this.metrics[x, y].Value;

        return
            (x == 0 || this.metrics[x - 1, y].MaxWest < value) ||
            (x == this.width - 1 || this.metrics[x + 1, y].MaxEast < value) ||
            (y == 0 || this.metrics[x, y - 1].MaxNorth < value) ||
            (y == this.height - 1 || this.metrics[x, y + 1].MaxSouth < value);
    }

    private int GetMaxWest(int x, int y, int value)
    {
        if (x == 0) return value;
        return Math.Max(this.metrics[x - 1, y].MaxWest, value);
    }

    private int GetMaxNorth(int x, int y, int value)
    {
        if (y == 0) return value;
        return Math.Max(this.metrics[x, y - 1].MaxNorth, value);
    }

    private int GetMaxEast(int x, int y, int value)
    {
        if (x == this.width - 1) return value;
        return Math.Max(this.metrics[x + 1, y].MaxEast, value);
    }

    private int GetMaxSouth(int x, int y, int value)
    {
        if (y == this.height - 1) return value;
        return Math.Max(this.metrics[x, y + 1].MaxSouth, value);
    }
}

internal record struct LocationMetric(int Value, int MaxNorth, int MaxEast, int MaxSouth, int MaxWest, int ScenicScore);
