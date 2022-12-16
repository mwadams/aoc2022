namespace Elf.WhoopDeWhoop;

public readonly record struct Node(char First, char Second)
{
    public override string ToString()
    {
        return $"{First}{Second}";
    }
}
