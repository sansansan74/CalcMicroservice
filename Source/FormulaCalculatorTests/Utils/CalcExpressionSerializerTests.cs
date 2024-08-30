using FormulaParser.Tree;

namespace FormulaCalculator.Utils.Tests
{
    [TestClass()]
    public class CalcExpressionSerializerTests
    {
        private const string JsonValue = "{\"oper_id\":\"Guid4\",\"oper\":\"add\",\"params\":[7.0,{\"oper_id\":\"Guid1\",\"oper\":\"mult\",\"params\":[1.0,2.0]},{\"oper_id\":\"Guid2\",\"oper\":\"divide\",\"params\":[3.0,4.0]},{\"oper_id\":\"Guid3\",\"oper\":\"subtract\",\"params\":[5.0,6.0]}]}";

        [TestMethod()]
        public void DeserializeFromJsonTest()
        {
            var op = CalcExpressionSerializer.DeserializeFromJson(JsonValue);
            Assert.IsNotNull(op);
            
            Assert.AreEqual("add", op.Operation);
            Assert.AreEqual(4, op.Items.Count);
            Assert.AreEqual(7.0, (op.Items[0] as CalcTreeLeaf)!.Value);
        }

        [TestMethod()]
        public void SerializeToJsonTest()
        {
            var op1 = new CalcTreeOperation("Guid1", "mult", new List<ICalcTreeItem>
            {
                new CalcTreeLeaf(1),
                new CalcTreeLeaf(2),
            });

            var op2 = new CalcTreeOperation("Guid2", "divide", new List<ICalcTreeItem>
            {
                new CalcTreeLeaf(3),
                new CalcTreeLeaf(4),
            });

            var op3 = new CalcTreeOperation("Guid3", "subtract", new List<ICalcTreeItem>
            {
                new CalcTreeLeaf(5),
                new CalcTreeLeaf(6),
            });

            var op4 = new CalcTreeOperation("Guid4", "add", new List<ICalcTreeItem>
            {
                new CalcTreeLeaf(7),
                op1,
                op2,
                op3,
            });

            var json = CalcExpressionSerializer.SerializeToJson(op4)
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("\t", "")
                .Replace(" ", "");

            Assert.AreEqual(JsonValue, json);
        }
    }
}