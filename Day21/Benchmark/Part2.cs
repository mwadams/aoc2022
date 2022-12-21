namespace Benchmarking;

using BenchmarkDotNet.Attributes;
using Elf.Shouting;

[MemoryDiagnoser]
public class Part2
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
        Console.WriteLine($"Pt2: {this.result}");
    }


    [Benchmark]
    public void RunPart2()
    {
        ElfAccumulatorPt2 accumulator = new(lines);
        this.result = accumulator.Process();
    }
}