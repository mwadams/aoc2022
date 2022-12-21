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
            
            // This turns it into - for a target of zero
            r.MakeSubtract(r.Left, r.Right);

            return root.Tuohs(h, 0);
        }
    }
}
