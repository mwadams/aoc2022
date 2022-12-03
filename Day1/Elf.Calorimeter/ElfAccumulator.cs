
/// <summary>
/// Accumulates the top N elves by calorie count.
/// </summary>
/// <remarks>
/// You pass it each line of an elf calorie record file, using <see cref="ProcessLine(string)"/>.
/// It builds a top-n set of <see cref="Elf"/> records, based on the number of calories each elf is carrying, in the
/// <see cref="Span{Elf}"/> you provide. This will be ordered by count, from lowest to highest.
/// <para>
/// Note that if there are fewer elf records in the input file than the number of slots available in the
/// output span, then the slots at at the lower indices will be <see cref="Elf.IsEmpty"/>
/// </para>
/// </remarks>
internal ref struct ElfAccumulator
{
    private int currentLine;
    private int startLineOfCurrentElf;
    private int currentElf;
    private int currentCalorieCount;

    private readonly Span<Elf> elves;

    public ElfAccumulator(ref Span<Elf> elves)
    {
        this.currentLine = 1;
        this.startLineOfCurrentElf = 1;
        this.currentElf = 0;
        this.currentCalorieCount = 0;
        this.elves = elves;
    }

    public void ProcessLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            this.TryInsertLastElf();

            this.currentCalorieCount = 0;
            this.currentElf++;


            // The "next" elf starts on the current line + 1.
            this.startLineOfCurrentElf = this.currentLine + 1;
        }
        else
        {
            if (int.TryParse(line, out int result))
            {
                this.currentCalorieCount += result;
            }
            else
            {
                Console.WriteLine($"Error processing line: {this.currentLine}. Unable to parse '{line}'");
            }
        }

        this.currentLine++;
    }

    /// <summary>
    /// Complete processing the file.
    /// </summary>
    /// <remarks>
    /// This will ensure the last record is processed.
    /// </remarks>
    public void FinishProcessing()
    {
        if (this.currentCalorieCount > 0)
        {
            this.TryInsertLastElf();
            this.currentCalorieCount = 0;
            this.currentElf++;
        }
    }

    private void TryInsertLastElf()
    {
        // We store the elves in sorted order
        int i = 0;
        // We can do a brute force iteration as we are expecting a low number of "top n" values
        // and brute force will be the most efficient.
        // We could switch to e.g. a binary search if elves.Length got very large
        for (; i < elves.Length; ++i)
        {
            if (elves[i].CalorieCount > this.currentCalorieCount)
            {
                // We need to insert immediately before this item
                break;
            }
        }

        int indexAtWhichToInsert = i - 1;
        if (indexAtWhichToInsert >= 0)
        {
            // This should be a very processor friendly copy; we're just blitting down the contents
            // of the array
            for (int index = 0; index < indexAtWhichToInsert; index++)
            {
                elves[index] = elves[index + 1];
            }

            elves[indexAtWhichToInsert] = new(this.currentCalorieCount, this.currentElf, this.startLineOfCurrentElf);
        }
    }
}
