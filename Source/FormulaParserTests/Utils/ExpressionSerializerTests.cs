namespace FormulaParser.Utils.Tests
{
    [TestClass()]
    public class ExpressionSerializerTests
    {
        [TestMethod()]
        public void DeserializeFromJsonTest()
        {
            string json =
                @"{
                    'value': {
                        'oper': 'mult',
                        'params': [
                            {
                                'oper': 'add',
                                'params': [
                                    1.0,
                                    -2.0
                                ]
                            },
                            {
                                'oper': 'subtract',
                                'params': [
                                    3.0,
                                    -4.0
                                ]
                            },
                            {
                                'oper': 'mult',
                                'params': [
                                    5.0,
                                    -6.0
                                ]
                            }

      
                        ]
                    }
                }";

            var wrapper = ExpressionSerializer.DeserializeFromJson(json);
            var val = wrapper!.Value.ToString()!.Replace(" ", "");
            Assert.AreEqual("mult(add(1,-2),subtract(3,-4),mult(5,-6))", val);
        }
    }
}