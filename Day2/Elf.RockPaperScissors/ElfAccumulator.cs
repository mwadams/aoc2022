namespace Elf.RockPaperScissors;

using System;

internal ref struct ElfAccumulator
{
    private int lineNumber;

    public ElfAccumulator()
    {
        this.TotalScore = 0;
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
            "A X" => 1 + 3, // Rock Rock => tie
            "A Y" => 2 + 6, // Rock Paper => win
            "A Z" => 3 + 0, // Rock Scissors => loss
            "B X" => 1 + 0, // Paper Rock => loss
            "B Y" => 2 + 3, // Paper Paper => tie
            "B Z" => 3 + 6, // Paper Scissors => win
            "C X" => 1 + 6, // Scissors Rock => win
            "C Y" => 2 + 0, // Scissors Paper => loss
            "C Z" => 3 + 3, // Scissors Scissors => tie
            _ => throw new InvalidOperationException($"Unexpected input on line {this.lineNumber}:{Environment.NewLine}{line}{Environment.NewLine}")
        };
    }
}