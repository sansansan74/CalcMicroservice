using FormulaParser.Tree;

namespace FormulaParser.Utils
{
    public static class TreePrinter
    {
        public static string Print(ITreeItem item)
        {
            return item switch
            {
                TreeLeaf leaf => leaf.Value.ToString(),
                TreeOperation operation => $"{operation.Operation}({string.Join(",", operation.Items.Select(Print))})",
                _ => throw new ArgumentException("Unknown TreeItem type")
            };
        }

        public static string Print(TreeWrapper item)
        {
            return  "{ 'value': {" + Print(item.Value) + "}}";
        }
    }
}


