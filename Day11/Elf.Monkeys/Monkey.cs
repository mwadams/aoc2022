namespace Elf.Monkeys;

using System.Numerics;

internal record struct Monkey(List<BigInteger> Items, Func<BigInteger, BigInteger> Operation, Predicate<BigInteger> Test, BigInteger TestValue, int TrueTarget, int FalseTarget)
{
    internal void AddItem(BigInteger worryLevel)
    {
        this.Items.Add(worryLevel);
    }
}
