using FormulaParser.Exceptions;
using FormulaParser.Tree;
using Operations;

namespace FormulaParser;

public class ExpressionParser
{
    Lexer lexer = null!;
    Token token => lexer.Token;

    public ITreeItem Parse(string expression)
    {
        lexer = new Lexer(expression);
        lexer.NextToken();

        if (token.Type == TokenType.End)
        {
            throw new ParseFormulaException("Expression is empty.");
        }

        var tree = ParseExpression();

        if (token.Type != TokenType.End)
        {
            throw new ParseFormulaException($"Unexpected token '{token.Value}' at position {Index}");
        }

        return tree;
    }

    int Index => lexer.Index;

    private ITreeItem ParseExpression()
    {
        var plusList = new List<ITreeItem>();
        var minusList = new List<ITreeItem>();

        Token unaryOperation = GetUnaryOperation();

        var term = ParseMult();
        AddExpressionToList(plusList, minusList, unaryOperation, term);

        while (token.Type == TokenType.Operation && (token.Value == "+" || token.Value == "-"))
        {
            var operation = token;
            lexer.NextToken();

            term = ParseMult();

            AddExpressionToList(plusList, minusList, operation, term);
        }

        if (minusList.Any() && plusList.Any())
        {
            return new TreeOperation(OperationNames.Subtract, new TreeOperation(OperationNames.Add, plusList), new TreeOperation(OperationNames.Add, minusList));
        }

        if (plusList.Any())
        {
            // we have no negative values, only positive
            return CheckSumWithOneOperand(plusList);
        }


        // we have only negative items
        // -x == (0-x)
        return new TreeOperation(OperationNames.Subtract, CreateZero(), CheckSumWithOneOperand(minusList));
    }

    private Token GetUnaryOperation()
    {
        if (token.Type == TokenType.Operation && (token.Value == "+" || token.Value == "-"))
        {
            // We have unary operation, like (-2)
            var operation = token;
            lexer.NextToken();
            return operation;
        }

        // +x == x
        // hack: if we not have unary "-", we always have unary "+"
        return new Token(TokenType.Operation, "+"); ;
    }

    private static void AddExpressionToList(List<ITreeItem> plusList, List<ITreeItem> minusList, Token operation, ITreeItem right)
    {
        if (operation.Value == "+")
        {
            plusList.Add(right);
            return;
        }

        if (right is TreeLeaf leaf)
        {
            // this is the leaf node (number) and operation minus:
            // simple invert value: change sign of number and add to plus list
            plusList.Add( new TreeLeaf(-leaf.Value));
            return;
        }

        // operation == "-" and node is not number
        minusList.Add(right);
    }

    private static ITreeItem CheckSumWithOneOperand(List<ITreeItem> plusItems) => CheckOperationWithOneOperand(plusItems, OperationNames.Add);

    private static ITreeItem CheckMultWithOneOperand(List<ITreeItem> multItems) => CheckOperationWithOneOperand(multItems, OperationNames.Mult);

    private static ITreeItem CheckOperationWithOneOperand(List<ITreeItem> items, string op)
    {
        if (items.Count == 1)
        {
            // we have only one item - no operation needed
            return items.First();
        }

        return new TreeOperation(op, items);
    }

    private static TreeLeaf CreateZero() => new TreeLeaf(0);


    private ITreeItem ParseMult()
    {
        var multItems = new List<ITreeItem>();
        var divItems = new List<ITreeItem>();

        var tree = ParseFactor();
        multItems.Add(tree);

        while (token.Type == TokenType.Operation && (token.Value == "*" || token.Value == "/"))
        {
            var op = token;
            lexer.NextToken();

            tree = ParseFactor();

            if (op.Value == "*")
            {
                multItems.Add(tree);
            }
            else {
                divItems.Add(tree);
            }
        }

        // multItems always have items. a/b or a*b  ==> a already have

        if (divItems.Any())
        {
            return new TreeOperation(OperationNames.Divide, CheckMultWithOneOperand(multItems), CheckMultWithOneOperand(divItems));
        }

        return CheckMultWithOneOperand(multItems);
    }

    private ITreeItem ParseFactor()
    {
        if (token.Type == TokenType.Numeric)
        {
            var leaf = new TreeLeaf(double.Parse(token.Value));
            lexer.NextToken();
            return leaf;
        }

        if (token.Type == TokenType.Special && token.Value == "(")
        {
            lexer.NextToken();
            var expr = ParseExpression();
            

            if (token.Type != TokenType.Special || token.Value != ")")
            {
                throw new ParseFormulaException($"Expected ')' at position {Index}");
            }

            lexer.NextToken();

            return expr;
        }

        if (token.Type == TokenType.Identifier)
        {
            var functionName = token.Value;
            lexer.NextToken(); // Move to the next token

            if (token.Type != TokenType.Special || token.Value != "(")
            {
                throw new ParseFormulaException($"Expected '(' after function name at position {lexer.Index}");
            }

            lexer.NextToken(); // Move to the next token
            var function = new TreeOperation(functionName);

            while (token.Type != TokenType.Special || token.Value != ")")
            {
                var arg = ParseExpression();
                function.Items.Add(arg);

                if (token.Type == TokenType.Special && token.Value == ",")
                {
                    lexer.NextToken(); // Move to the next token
                    continue;
                }
            }

            if (token.Type != TokenType.Special || token.Value != ")")
            {
                throw new ParseFormulaException($"Expected ')' at position {lexer.Index}");
            }

            lexer.NextToken(); // Move to the next token
            return function;
        }

        throw new ParseFormulaException($"Unexpected token '{token.Value}' at position {Index}");
    }
}

