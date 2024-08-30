namespace FormulaParser.Tree;

public class CalcTreeLeaf : ICalcTreeItem
{
    public double Value { get; set; }

    public CalcTreeLeaf(double value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
