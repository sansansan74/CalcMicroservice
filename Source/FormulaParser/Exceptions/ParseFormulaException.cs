using System;

namespace FormulaParser.Exceptions
{

    public class ParseFormulaException : Exception
    {
        public ParseFormulaException() { }
        public ParseFormulaException(string message) : base(message) { }
        public ParseFormulaException(string message, Exception inner) : base(message, inner) { }
    }

}