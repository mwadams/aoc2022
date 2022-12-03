
/// <summary>
/// Holds the information about an elf.
/// </summary>
/// <param name="CalorieCount">The calorie count for the elf.</param>
/// <param name="Index">The index of the elf in the order in which it appears in the input file.</param>
/// <param name="StartLine">The start line of the elf's record in the input file.</param>
internal readonly record struct Elf(int CalorieCount, int Index, int StartLine)
{
    /// <summary>
    /// Gets a value indicating whether this elf record is empty.
    /// </summary>
    public bool IsEmpty => this.StartLine == 0;
}
