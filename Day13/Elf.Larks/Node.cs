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
    private static readonly List<Node> Empty = new();

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

    private static Status CompareListifiedLeftToListRight(in Node lhs, in Node rhs)
    {
        if (rhs.children.Count == 0)
        {
            return Status.OutOfOrder;
        }

        var result = lhs.Compare(rhs.children[0]);
        if (result != Status.Continue)
        {
            return result;
        }

        if (rhs.children.Count >= 1)
        {
            return Status.InOrder;
        }

        return Status.Continue;
    }

    private static Status CompareListLeftToListifiedRight(in Node lhs, in Node rhs)
    {
        int leftIndex = 0;
        int rightIndex = 0;
        while (leftIndex < lhs.children.Count)
        {
            if (rightIndex >= 1)
            {
                return Status.OutOfOrder;
            }

            var result = lhs.children[leftIndex].Compare(rhs);
            if (result != Status.Continue)
            {
                return result;
            }

            ++leftIndex;
            ++rightIndex;
        }

        if (rightIndex < 1)
        {
            return Status.InOrder;
        }

        return Status.Continue;
    }

    public Status Compare(in Node rhs)
    {
        if (this.value is int lhsValue)
        {
            if (rhs.value is int rhsValue)
            {
                return CompareIntegers(lhsValue, rhsValue);
            }
            else
            {
                return CompareListifiedLeftToListRight(this, rhs);
            }
        }
        else if (rhs.value is int)
        {
            return CompareListLeftToListifiedRight(this, rhs);
        }
        else
        {
            int leftIndex = 0;
            int rightIndex = 0;
            while (leftIndex < this.children.Count)
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
            foreach (var child in this.children)
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
            str.Append(']');
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
