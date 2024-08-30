namespace FormulaParser.Tree;

public class CalcTreeOperation : ICalcTreeItem
{
    public string OperationId { get; set; }
    public string Operation { get; set; }
    public List<ICalcTreeItem> Items { get; set; }

    public CalcTreeOperation(string operationId, string operation, List<ICalcTreeItem> items = null!)
    {
        Operation = operation;
        OperationId = operationId;
        Items = items ?? new List<ICalcTreeItem>();
    }

    public override string ToString()
    {
        var itemsStr = string.Join(", ", Items.Select(i => i.ToString()));
        return $"{Operation}({itemsStr})";
    }
}

