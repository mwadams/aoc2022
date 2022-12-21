namespace Elf.Shouting
{
    public class MonkeyGraphBase
    {
        private readonly Dictionary<string, Monkey> namedMonkeys = new();
        private readonly bool ignoreHuman;
        protected Monkey? root;
        protected Monkey? humn;

        protected MonkeyGraphBase(bool ignoreHuman = false)
        {
            this.ignoreHuman = ignoreHuman;
        }

        public void AddMonkey(ReadOnlySpan<char> line)
        {
            Monkey added;
            var name = line[..4].ToString();
            if (line[6] >= '0' && line[6] <= '9')
            {
                if (ignoreHuman && name == "humn")
                {
                    // A human is an uninitialized monkey
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
