using FormulaParser.Utils;

namespace FormulaParser.Tests
{
    [TestClass]
    public class ExpressionParserTests
    {
        private ExpressionParser _parser = null!;

        [TestInitialize]
        public void Setup()
        {
            _parser = new ExpressionParser();
        }




        [DataTestMethod]
        [DataRow("1", "1")]
        [DataRow("1.2", "1.2")]
        [DataRow("-1.2", "-1.2")]
        [DataRow(".2", "0.2")]
        [DataRow("-.2", "-0.2")]
        [DataRow("-1", "-1")]
        [DataRow("+1", "1")]
        [DataRow("1+2+3", "add(1,2,3)")]
        [DataRow("1-2", "add(1,-2)")]
        [DataRow("1-2+3-4", "add(1,-2,3,-4)")]
        [DataRow(" ( 1) ", "1")]
        [DataRow("+(1)", "1")]
        [DataRow("(+1)", "1")]
        [DataRow("+(+1)", "1")]
        [DataRow("(-1)", "-1")]
        [DataRow("-(1)", "-1")]
        [DataRow("-(-1)", "1")]
        [DataRow("2*3", "mult(2,3)")]
        [DataRow("2/3/4", "divide(2,mult(3,4))")]
        [DataRow("1+2*3", "add(1,mult(2,3))")]
        [DataRow("(1+2)*3/(4-5)", "divide(mult(add(1,2),3),add(4,-5))")]
        [DataRow("-(1+2)*3/(4-5)", "subtract(0,divide(mult(add(1,2),3),add(4,-5)))")]
        [DataRow("exp(2,3)", "exp(2,3)")]
        [DataRow("add(2, 3, 4)", "add(2,3,4)")]
        [DataRow("Pi()", "pi()")]

        public void Parse_ValidExpressions_ReturnsCorrectTree(string expression, string expected)
        {
            var result = _parser.Parse(expression);
            string resultString = TreePrinter.Print(result);
            Assert.AreEqual(expected, resultString);
        }


        [DataTestMethod]
        [DataRow("1a", "Unexpected token 'a' at position 2")]
        [DataRow(" 0.2.2 3 ", "Unexpected character '.' at position 4")]
        [DataRow(" 2 3 ", "Unexpected token '3' at position 4")]
        [DataRow("(1+2) 12", "Unexpected token '12' at position 8")]
        [DataRow("1**2", "Unexpected token '*' at position 3")]
        [DataRow("--1", "Unexpected token '-' at position 2")]
        [DataRow("2+-3", "Unexpected token '-' at position 3")]
        [DataRow("2--3", "Unexpected token '-' at position 3")]
        [DataRow("1++2", "Unexpected token '+' at position 3")]
        [DataRow("1//2", "Unexpected token '/' at position 3")]
        [DataRow("1+2) 12", "Unexpected token ')' at position 4")]
        [DataRow("(1+2 12", "Expected ')' at position 7")]
        [DataRow("(1+2+12", "Expected ')' at position 7")]
        public void Parse_InvalidExpressions_ThrowsException(string expression, string expectedErrorMessage)
        {
            try
            {
                _parser.Parse(expression);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual(expectedErrorMessage, ex.Message);
            }
        }

    }
}
