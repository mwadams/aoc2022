namespace Elf.Shouting
{
    public class Monkey
    {
        private enum Operation { Addition, Multiplication, Subtraction, Division };

        private long? number;
        private Monkey? left;
        private Monkey? right;
        private Func<Monkey, long>? deferredScream;
        private bool? leftContainsHuman;
        private bool? rightContainsHuman;
        private Operation operation;


        public long Number => this.number ??= deferredScream!(this);
        public Monkey Left => this.left ?? throw new InvalidOperationException();
        public Monkey Right => this.right ?? throw new InvalidOperationException();

        public bool CanShout => number is long || (left is Monkey l && l.CanShout && right is Monkey r && r.CanShout);

        public long Shout()
        {
            Stack<Action> howlers = new();
            this.PrepareToShout(howlers);

            while (howlers.Count > 0)
            {
                howlers.Pop()();
            }

            return this.Number;
        }

        public void MakeAdd(Monkey left, Monkey right)
        {
            this.operation = Operation.Addition;
            this.left = left;
            this.right = right;
            this.deferredScream = static (m) => m.left!.Number + m.right!.Number;
        }

        public void MakeSubtract(Monkey left, Monkey right)
        {
            this.operation = Operation.Subtraction;
            this.left = left;
            this.right = right;
            this.deferredScream = static (m) => (m.left!.Number - m.right!.Number);
        }

        public void MakeMultiplication(Monkey left, Monkey right)
        {
            this.operation = Operation.Multiplication;
            this.left = left;
            this.right = right;
            this.deferredScream = static (m) => (m.left!.Number * m.right!.Number);
        }

        public void MakeDivision(Monkey left, Monkey right)
        {
            this.operation = Operation.Division;
            this.left = left;
            this.right = right;
            this.deferredScream = static (m) => (m.left!.Number / m.right!.Number);
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
            // Inverse operations
            return operation switch
            {
                Operation.Addition => requiredTotal -= number,
                Operation.Subtraction => number - requiredTotal,
                Operation.Multiplication => requiredTotal / number,
                Operation.Division => number / requiredTotal,
                _ => throw new NotSupportedException()
            };
        }

        public long GetExpectedFromRequiredAndRight(long requiredTotal, long number)
        {
            // Inverse operations
            return operation switch
            {
                Operation.Addition => requiredTotal -= number,
                Operation.Subtraction => number + requiredTotal,
                Operation.Multiplication => requiredTotal / number,
                Operation.Division => number * requiredTotal,
                _ => throw new NotSupportedException()
            };
        }

        private void PrepareToShout(Stack<Action> howlers)
        {
            if (CanShout)
            {
                if (number is not long n)
                {
                    number = deferredScream!(this);
                }
            }
            else
            {
                howlers.Push(() => this.number = deferredScream!(this));
                if (left is Monkey l && right is Monkey r)
                {
                    l.PrepareToShout(howlers);
                    r.PrepareToShout(howlers);
                }
            }
        }
    }
}
