using FormulaParser.Exceptions;
using System.Text;

namespace FormulaParser;


sealed class Lexer
{
    private readonly string _expression;
    private int _index;
    Token token = null!;

    public Lexer(string expression)
    {
        _expression = expression;
        _index = 0;
    }

    public int Index => _index;
    public Token Token => token;

    public void NextToken()
    {
        SkipWhitespace();

        if (Eof())
        {
            token = new Token(TokenType.End, string.Empty);
            return ;
        }

        char currentChar = CurrentChar;

        if (char.IsDigit(currentChar) || currentChar == '.')
        {
            ParseNumber();
            return;
        }

        if (IsOperator(currentChar))
        {
            _index++;
            token = new Token(TokenType.Operation, currentChar.ToString());
            return;
        }

        if (IsSpecial(currentChar))
        {
            _index++;
            token = new Token(TokenType.Special, currentChar.ToString());
            return;
        }

        if (char.IsAsciiLetter(currentChar))
        {
            ParseIdentifier();
            return;
        }

        throw new ParseFormulaException($"Unexpected character '{currentChar}' at position {_index}");
    }

    private void ParseIdentifier()
    {
        StringBuilder identifier = new StringBuilder();

        for (; NotEof() && char.IsAsciiLetterOrDigit(CurrentChar); _index++)
        {
            identifier.Append(CurrentChar);
        }

        token = new Token(TokenType.Identifier, identifier.ToString().ToLower());
    }
    private void SkipWhitespace()
    {
        for (; NotEof() && char.IsWhiteSpace(CurrentChar); _index++)
            ;
    }

    private bool NotEof() => _index < _expression.Length;
    private bool Eof() => _index >= _expression.Length;

    private void ParseNumber()
    {
        StringBuilder number = new StringBuilder();
        int dotCount = 0;

        for (; NotEof() && (char.IsDigit(CurrentChar) || CurrentChar == '.'); _index++)
        {
            if (CurrentChar == '.')
            {
                if (dotCount > 0)
                {
                    throw new Exception($"Unexpected character '.' at position {_index}");
                }
                dotCount++;
            }
            number.Append(CurrentChar);
        }

        token = new Token(TokenType.Numeric, number.ToString());
    }

    private char CurrentChar => _expression[_index];
        

    private static bool IsOperator(char ch) => ch == '+' || ch == '-' || ch == '*' || ch == '/';

    private static bool IsSpecial(char ch) => ch == '(' || ch == ')' || ch == ',';
}

