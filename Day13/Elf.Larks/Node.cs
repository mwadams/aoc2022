namespace Elf.Larks;

using System.Text;

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

    public bool IsMarker { get; }

    public Node(int value, bool isMarker)
    {
        this.children = Empty;
        this.value = value;
        IsMarker = isMarker;
    }

    public Node(List<Node> children, bool isMarker)
    {
        this.children = children;
        this.value = null;
        this.IsMarker = isMarker;
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
                return new Node(new List<Node> { this }, false).Compare(rhs);
            }
        }
        else if (rhs.value is int)
        {
            return this.Compare(new Node(new List<Node> { rhs }, false));
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

    public override string ToString()
    {
        StringBuilder str = new();
        this.WriteTo(str);
        return str.ToString();
    }
    private void WriteTo(StringBuilder str)
    {
        if (this.value is int v)
        {
            str.Append(v);
        }
        else
        {
            str.Append('[');
            bool first = true;
            foreach(var child in this.children)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    str.Append(',');
                }

                child.WriteTo(str);
            }
            str.Append("]");
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
