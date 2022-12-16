namespace Elf.WhoopDeWhoop;

public readonly record struct Item(long OpenedValves, int RemainingTime, Node Current, long TotalFlow);
