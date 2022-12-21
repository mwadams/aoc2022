namespace Benchmarking;

using BenchmarkDotNet.Attributes;
using Elf.Larks;

/// <summary>
/// Construct elements from a JSON element.
/// </summary>
[MemoryDiagnoser]
public class Part1
{
    private string[] lines = Array.Empty<string>();
    private long result;

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.lines = File.ReadAllLines("./input.txt");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Console.WriteLine($"Pt1: {this.result}");
    }

    [Benchmark]
    public void RunPart1()
    {
        ElfAccumulatorPt1 accumulator = new(lines);
        this.result = accumulator.ProcessLines();
    }
}