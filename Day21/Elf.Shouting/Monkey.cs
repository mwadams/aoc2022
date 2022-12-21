namespace Elf.Shouting
{
    public class Monkey
    {
        private enum Operation { Addition, Multiplication, Subtraction, Division };

        private long? number;
        private Monkey? left;
        private Monkey? right;
        private Func<long, long, long>? screamOperation;
        private Func<long, long, long>? maercsOperationFromLeft;
        private Func<long, long, long>? maercsOperationFromRight;
        private bool? leftContainsHuman;
        private bool? rightContainsHuman;


        public Monkey Left => this.left ?? throw new InvalidOperationException();

        public Monkey Right => this.right ?? throw new InvalidOperationException();

        public long Shout()
        {
            return this.number ??= screamOperation!(left!.Shout(), right!.Shout());
        }

        public long Tuohs(Monkey human, long requiredTotal)
        {
            if (this.LeftContainsHuman(human))
            {
                return left!.Tuohs(human, GetExpectedFromRequiredAndRight(requiredTotal, right!.Shout()));
            }
            else if (this.RightContainsHuman(human))
            {
                return right!.Tuohs(human, GetExpectedFromRequiredAndLeft(requiredTotal, left!.Shout()));
            }

            return requiredTotal;
        }

        public void MakeAdd(Monkey left, Monkey right)
        {
            this.left = left;
            this.right = right;
            this.screamOperation = static (l, r) => l + r;
            this.maercsOperationFromLeft = static (t, l) => t - l;
            this.maercsOperationFromRight = static (t, r) => t - r;
        }

        public void MakeSubtract(Monkey left, Monkey right)
        {
            this.left = left;
            this.right = right;
            this.screamOperation = static (l, r) => (l - r);
            this.maercsOperationFromLeft = static (t, l) => l - t;
            this.maercsOperationFromRight = static (t, r) => r + t;
        }

        public void MakeMultiplication(Monkey left, Monkey right)
        {
            this.left = left;
            this.right = right;
            this.screamOperation = static (l, r) => (l * r);
            this.maercsOperationFromLeft = static (t, l) => t / l;
            this.maercsOperationFromRight = static (t, r) => t / r;
        }

        public void MakeDivision(Monkey left, Monkey right)
        {
            this.left = left;
            this.right = right;
            this.screamOperation = static (l, r) => (l / r);
            this.maercsOperationFromLeft = static (t, l) => l / t;
            this.maercsOperationFromRight = static (t, r) => r * t;
        }

        public void SetNumber(long number)
        {
            this.number = number;
        }

        public bool LeftContainsHuman(Monkey? monkey)
        {
            return this.leftContainsHuman ??= (this.left == monkey || (this.left is Monkey m && (m.LeftContainsHuman(monkey) || m.RightContainsHuman(monkey))));
        }

        public bool RightContainsHuman(Monkey? monkey)
        {
            return this.rightContainsHuman ??= (this.right == monkey || (this.right is Monkey m && (m.LeftContainsHuman(monkey) || m.RightContainsHuman(monkey))));
        }

        public long GetExpectedFromRequiredAndLeft(long requiredTotal, long number)
        {
            return maercsOperationFromLeft!(requiredTotal, number);
        }

        public long GetExpectedFromRequiredAndRight(long requiredTotal, long number)
        {
            return maercsOperationFromRight!(requiredTotal, number);
        }
    }
}
