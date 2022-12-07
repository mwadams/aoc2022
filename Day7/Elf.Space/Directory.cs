namespace Elf.Space;

public class Directory
{
    private readonly List<Directory> children = new();
    private int? totalSize;
    private int size = 0;

    public Directory()
    {
    }

    // Memoize the total size
    public int TotalSize => (this.totalSize ??= this.children.Sum(c => c.TotalSize) + this.size);
    
    public void AddFile(int size)
    {
        this.size += size;
    }

    public Directory AddChild()
    {
        Directory result = new Directory();
        this.children.Add(result);
        return result;
    }
}