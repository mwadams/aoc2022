namespace Elf.Monkeys;
internal record struct Monkey(List<ulong> Items, Func<ulong, ulong> Operation, Predicate<ulong> Test, ulong TestValue, int TrueTarget, int FalseTarget);