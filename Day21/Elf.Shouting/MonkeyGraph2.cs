namespace Elf.Shouting
{
    using System.Data;

    public class MonkeyGraph2
    {
        private readonly Dictionary<string, Monkey> namedMonkeys = new();
        private Monkey? root;
        private Monkey? humn;

        public long Shout()
        {
            Monkey r = root ?? throw new InvalidOperationException();
            Monkey h = humn ?? throw new InvalidOperationException();

            long requiredTotal = 0;
            while (r != humn)
            {
                if (r.LeftContainsHuman(h))
                {
                    Shout(r.Right!);
                    requiredTotal = r.GetExpectedFromRequired(requiredTotal);
                    if (r == root)
                    {
                        // Invert for the root to make our equality operation
                        requiredTotal = -requiredTotal;
                    }

                    r = r.Left!;
                }
                else if (r.RightContainsHuman(h))
                {
                    Shout(r.Left!);
                    requiredTotal = r.GetExpectedFromRequired(requiredTotal);
                    if (r == root)
                    {
                        // Invert for the root to make our equality operation
                        requiredTotal = -requiredTotal;
                    }

                    r = r.Right!;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return requiredTotal;
        }

        private static long Shout(Monkey monkey)
        {
            Stack<Action> howlers = new();
            monkey.PrepareToShout(howlers);

            while (howlers.Count > 0)
            {
                howlers.Pop()();
            }

            return monkey.Number;
        }

        public void ProcessLine(ReadOnlySpan<char> line)
        {
            Monkey added;
            var name = line[..4].ToString();
            if (line[6] >= '0' && line[6] <= '9')
            {
                if (name == "humn")
                {
                    // It's all null
                    added = GetOrAddMonkey(name);
                    humn = added;
                }
                else
                {

                    added = AddMonkey(name, long.Parse(line[6..]));
                }

            }
            else
            {
                switch (line[11])
                {
                    case '+':
                        added = AddAdditionMonkey(name, line[6..10].ToString(), line[13..].ToString());
                        break;
                    case '-':
                        added = AddSubtractionMonkey(name, line[6..10].ToString(), line[13..].ToString());
                        break;
                    case '*':
                        added = AddMultiplicationMonkey(name, line[6..10].ToString(), line[13..].ToString());
                        break;
                    case '/':
                        added = AddDivisionMonkey(name, line[6..10].ToString(), line[13..].ToString());
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            if (name == "root")
            {
                root = added;
            }
        }

        private Monkey AddAdditionMonkey(string name, string lhs, string rhs)
        {
            Monkey monkey = GetOrAddMonkey(name);
            Monkey leftMonkey = GetOrAddMonkey(lhs);
            Monkey rightMonkey = GetOrAddMonkey(rhs);
            monkey.MakeAdd(leftMonkey, rightMonkey);
            return monkey;
        }

        private Monkey AddSubtractionMonkey(string name, string lhs, string rhs)
        {
            Monkey monkey = GetOrAddMonkey(name);
            Monkey leftMonkey = GetOrAddMonkey(lhs);
            Monkey rightMonkey = GetOrAddMonkey(rhs);
            monkey.MakeSubtract(leftMonkey, rightMonkey);
            return monkey;
        }

        private Monkey AddMultiplicationMonkey(string name, string lhs, string rhs)
        {
            Monkey monkey = GetOrAddMonkey(name);
            Monkey leftMonkey = GetOrAddMonkey(lhs);
            Monkey rightMonkey = GetOrAddMonkey(rhs);
            monkey.MakeMultiplication(leftMonkey, rightMonkey);
            return monkey;
        }

        private Monkey AddDivisionMonkey(string name, string lhs, string rhs)
        {
            Monkey monkey = GetOrAddMonkey(name);
            Monkey leftMonkey = GetOrAddMonkey(lhs);
            Monkey rightMonkey = GetOrAddMonkey(rhs);
            monkey.MakeDivision(leftMonkey, rightMonkey);
            return monkey;
        }

        private Monkey GetOrAddMonkey(string name)
        {
            if (!namedMonkeys.TryGetValue(name, out Monkey? monkey))
            {
                monkey = new Monkey();
                namedMonkeys.Add(name, monkey);
            }

            return monkey;
        }

        private Monkey AddMonkey(string name, long number)
        {
            Monkey monkey = GetOrAddMonkey(name);
            monkey.SetNumber(number);
            return monkey;
        }
    }
}
