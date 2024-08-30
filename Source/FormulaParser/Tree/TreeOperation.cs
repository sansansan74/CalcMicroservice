namespace FormulaParser.Tree;

public class TreeOperation : ITreeItem
{
    public string Operation { get; set; }
    public List<ITreeItem> Items { get; set; }

    public TreeOperation(string operation, List<ITreeItem> items = null!)
    {
        Operation = operation;
        Items = items ?? new List<ITreeItem>();
    }

    public TreeOperation(string operation, ITreeItem op1, ITreeItem op2)
    {
        Operation = operation;
        Items = new List<ITreeItem>() { op1, op2 };
    }

    public override string ToString()
    {
        var itemsStr = string.Join(", ", Items.Select(i => i.ToString()));
        return $"{Operation}({itemsStr})";
    }
}

