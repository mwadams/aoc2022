namespace Elf.Obsidian;

[Flags]
public enum State : byte
{
    Exists = 0b0000_0001,
    Face1 = 0b0000_0010,
    Face2 = 0b0000_0100,
    Face3 = 0b0000_1000,
    Face4 = 0b0001_0000,
    Face5 = 0b0010_0000,
    Face6 = 0b0100_0000,
    ConnectsToEdge = 0b1000_0000,
    Faces = Face1 | Face2 | Face3 | Face4 | Face5 | Face6,
}