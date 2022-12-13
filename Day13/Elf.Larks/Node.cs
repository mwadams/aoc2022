namespace Elf.Larks;

public enum Status
{
    Continue,
    InOrder,
    OutOfOrder
}

public readonly struct Node
{
    private static readonly List<Node> Empty = new List<Node>();

    private readonly List<Node> children;
    private readonly int? value;

    public Node(int value)
    {
        this.children = Empty;
        this.value = value;
    }

    public Node(List<Node> children)
    {
        this.children = children;
        this.value = null;
    }


    public Status Compare(Node rhs)
    {
        if (this.value is int lhsValue)
        {
            if (rhs.value is int rhsValue)
            {
                return CompareIntegers(lhsValue, rhsValue);
            }
            else
            {
                return new Node(new List<Node> { this }).Compare(rhs);
            }
        }
        else if (rhs.value is int rhsValue)
        {
            return this.Compare(new Node(new List<Node> { rhs }));
        }
        else
        {
            int leftIndex = 0;
            int rightIndex = 0;
            while(leftIndex < this.children.Count)
            {
                if (rightIndex >= rhs.children.Count)
                {
                    return Status.OutOfOrder;
                }

                var result = this.children[leftIndex].Compare(rhs.children[rightIndex]);
                if (result != Status.Continue)
                {
                    return result;
                }

                ++leftIndex;
                ++rightIndex;
            }

            if (rightIndex < rhs.children.Count)
            {
                return Status.InOrder;
            }

            return Status.Continue;
        }
    }

    private static Status CompareIntegers(int lhsValue, int rhsValue)
    {
        if (lhsValue < rhsValue)
        {
            return Status.InOrder;
        }
        else if (rhsValue < lhsValue)
        {
            return Status.OutOfOrder;
        }

        return Status.Continue;
    }
}
