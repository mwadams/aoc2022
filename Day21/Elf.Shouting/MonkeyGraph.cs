namespace Elf.Shouting
{
    public class MonkeyGraph : MonkeyGraphBase
    {
        public long Process()
        {
            return root!.Shout();
        }
    }
}
