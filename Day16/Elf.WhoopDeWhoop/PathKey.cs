namespace Elf.WhoopDeWhoop;

public readonly record struct PathKey(Node First, Node Second)
{
    public override string ToString()
    {
        return $"{First}:{Second}";
    }
}
