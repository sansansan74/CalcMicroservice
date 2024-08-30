using Newtonsoft.Json;

namespace FormulaParser.Tree
{
    /// <summary>
    /// This class used to wrapping Tree Expression for serialization to JSon
    /// 20 - is correct expression for expression tree, but 20 is not correct JSon, because top item in JSon must be object or array
    /// We need the object for wrapping the tree expression
    /// </summary>
    /// <param name="treeItem"></param>
    public class TreeWrapper(ITreeItem value)
    {
        [JsonProperty("value")]
        public ITreeItem Value { get; set; } = value;
    }
}
