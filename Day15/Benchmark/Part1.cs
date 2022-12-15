namespace Benchmarking;

using BenchmarkDotNet.Attributes;
using Elf.Hoopla;

/// <summary>
/// Construct elements from a JSON element.
/// </summary>
[MemoryDiagnoser]
public class Part1
{
    private string[] lines = Array.Empty<string>();
    private long result;

    /// <summary>
    /// Global setup.
    /// </summary>
    /// <returns>A <see cref="Task"/> which completes once cleanup is complete.</returns>
    [GlobalSetup]
    public void GlobalSetup()
    {
        this.lines = File.ReadAllLines("./input.txt");
    }

    /// <summary>
    /// Global setup.
    /// </summary>
    /// <returns>A <see cref="Task"/> which completes once cleanup is complete.</returns>
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        Console.WriteLine($"Pt1: {this.result}");
    }

    /// <summary>
    /// Resolve a URI from a template and parameter values using Corvus.UriTemplateResolver.
    /// </summary>
    [Benchmark]
    public void RunPart1()
    {
        ElfAccumulator accumulator = new(lines);
        this.result = accumulator.Process(2000000);
    }
}