namespace Elf.WhoopDeWhoop;

public readonly record struct VisitedKey(Node Current, int RemainingTime, long OpenedValves, long TotalFlow)
{
    public override string ToString()
    {
        return $"{Current}:{RemainingTime}:{OpenedValves}:{TotalFlow}";
    }
}
