namespace Elf.Space;

public class Directory
{
    private readonly Dictionary<string, Directory> children = new();
    private readonly string name;
    private int? totalSize;
    private int size = 0;

    public Directory(string name)
    {
        this.name = name;
    }

    // Memoize the total size
    public int TotalSize => (this.totalSize ??= this.children.Sum(c => c.Value.TotalSize) + this.size);
    
    public void AddFile(int size)
    {
        this.size += size;
    }

    public Directory GetOrAddChild(string name)
    {
        if (this.children.TryGetValue(name,out Directory? value))
        {
            return value;
        }

        Directory result = new Directory(name);
        this.children.Add(name, result);
        return result;
    }
}