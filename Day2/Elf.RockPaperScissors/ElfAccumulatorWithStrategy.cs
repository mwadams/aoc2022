namespace Elf.RockPaperScissors;

using System;

internal ref struct ElfAccumulatorWithStrategy
{
    private int lineNumber;

    public ElfAccumulatorWithStrategy()
    {
        this.TotalScore = 0;
        this.lineNumber = 0;
    }

    public int TotalScore { get; private set; }

    public void ProcessLine(string line)
    {
        this.lineNumber++;
        this.TotalScore += this.ScoreLine(line);
    }

    private int ScoreLine(string line)
    {
        return line switch
        {
            "A X" => 0 + 3, // Rock Lose => Scissors
            "A Y" => 3 + 1, // Rock Draw => Rock
            "A Z" => 6 + 2, // Rock Win => Paper
            "B X" => 0 + 1, // Paper Lose => Rock
            "B Y" => 3 + 2, // Paper Draw => Paper
            "B Z" => 6 + 3, // Paper Win => Scissors
            "C X" => 0 + 2, // Scissors Lose => Paper
            "C Y" => 3 + 3, // Scissors Draw => Scissors
            "C Z" => 6 + 1, // Scissors Win => Rock
            _ => throw new InvalidOperationException($"Unexpected input on line {this.lineNumber}:{Environment.NewLine}{line}{Environment.NewLine}")
        };
    }
}