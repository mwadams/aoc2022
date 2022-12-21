namespace Elf.Shouting
{
    public class MonkeyGraph2 : MonkeyGraphBase
    {
        public MonkeyGraph2()
            : base (ignoreHuman: true) 
        {
        }

        public long Process()
        {
            Monkey r = root ?? throw new InvalidOperationException();
            Monkey h = humn ?? throw new InvalidOperationException();

            long requiredTotal = 0;
            while (r != humn)
            {
                if (r.LeftContainsHuman(h))
                {
                    long b = r.Right!.Shout();
                    requiredTotal = r.GetExpectedFromRequiredAndRight(requiredTotal, b);
                    if (r == root)
                    {
                        // Invert for the root to make our equality operation
                        requiredTotal = -requiredTotal;
                    }

                    r = r.Left!;
                }
                else if (r.RightContainsHuman(h))
                {
                    long a = r.Left!.Shout();
                    requiredTotal = r.GetExpectedFromRequiredAndLeft(requiredTotal, a);
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
    }
}
