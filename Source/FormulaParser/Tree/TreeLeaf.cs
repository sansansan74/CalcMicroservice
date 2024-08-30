namespace FormulaParser.Tree;

public class TreeLeaf : ITreeItem
{
    public double Value { get; set; }

    public TreeLeaf(double value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
