using FormulaParser.Tree;

namespace FormulaCalculator.Utils
{
    public class CalcTreeUtils
    {
        public static ICalcTreeItem CreateTree(ITreeItem treeItem)
        {
            switch (treeItem)
            {
                case TreeLeaf leaf:
                    return new CalcTreeLeaf(leaf.Value);
                case TreeOperation treeOperation:
                    var calcOperation = new CalcTreeOperation(
                        Guid.NewGuid().ToString(),
                        treeOperation.Operation,
                        treeOperation.Items.Select(CreateTree).ToList());

                    calcOperation.OperationId = Guid.NewGuid().ToString();

                    return calcOperation;
                default:
                    throw new InvalidOperationException("Unknown TreeItem type");
            };
        }

        public static ITreeItem CreateTree(ICalcTreeItem treeItem)
        {
            switch (treeItem)
            {
                case CalcTreeLeaf leaf:
                    return new TreeLeaf(leaf.Value);
                case CalcTreeOperation treeOperation:
                    var calcOperation = new TreeOperation(
                        treeOperation.Operation,
                        treeOperation.Items.Select(CreateTree).ToList());

                    return calcOperation;
                default:
                    throw new InvalidOperationException("Unknown TreeItem type");
            };
        }

        public static CalcTreeOperation FindCommandForCalculate(CalcTreeOperation calcTree)
        {
            foreach (var node in calcTree.Items)
            {
                // if we have complex node (operation), then we are not command for calculate
                // command for calculate has only leafs as sons
                if (node is CalcTreeOperation op)
                    return FindCommandForCalculate(op);
            }

            return calcTree;
        }

        public static ICalcTreeItem ReplaceEvaluatedOperationInTree(CalcTreeOperation tree, string operationId, double value)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            // Recursive function to search and replace the node
            (ICalcTreeItem Tree, bool IsReplaced) ReplaceNode(ICalcTreeItem currentNode)
            {
                if (currentNode is not CalcTreeOperation operationNode)
                {
                    return (currentNode, false);
                }

                if (operationNode.OperationId == operationId)
                {
                    // Replace the node with a new leaf
                    return (new CalcTreeLeaf(value), true);
                }

                bool isReplaced = false;
                for (int i = 0; i < operationNode.Items.Count; i++)
                {
                    var (newChild, childReplaced) = ReplaceNode(operationNode.Items[i]);

                    if (childReplaced)
                    {
                        operationNode.Items[i] = newChild;
                        isReplaced = true;
                        break;
                    }
                }

                return (operationNode, isReplaced);
            }

            var (newTree, isReplaced) = ReplaceNode(tree);

            if (!isReplaced)
            {
                throw new Exception($"There is no Operation with OperationId={operationId} in expression tree");
            }

            return newTree;
        }
    }
}
