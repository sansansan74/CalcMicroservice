using FormulaParser.Tree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MathExpressionParser.Utils
{
    /// <summary>
    /// Class convert to JSon nodes of tree expression: TreeLeaf and TreeOperation
    /// </summary>

    /*
    sample of tree:
    
    1)
    math expression: 2+3

    tree view:
    └─add
        ├─  2.0
        └─  3.0

    tree expression
    {
        "oper": "add",
        "params": [
            2.0,
            3.0
        ]
    }


    2)
    math expression: (2-3)*4
    
    tree view:
    └─mult
        ├─add
        │ ├─  2
        │ └─  -3
        └─  4

    tree expression:
    {
      "value": {
        "oper": "mult",
        "params": [
          {
            "oper": "add",
            "params": [
                  2.0,
                  -3.0
            ]
          },
          4.0
        ]
      }
    }


*/
    class TreeItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ITreeItem).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            switch (value)
            {
                case TreeLeaf leaf:
                    // this is not an object. This is simple value
                    writer.WriteValue(leaf.Value);
                    return;
                case TreeOperation operation:
                    WriteJsonTreeOperatin(writer, serializer, operation);
                    return;
                default:
                    throw new InvalidOperationException("Unknown TreeItem type");
            };
        }

        private static void WriteJsonTreeOperatin(JsonWriter writer, JsonSerializer serializer, TreeOperation operation)
        {
            /*
                write operation node. node contains operation name (add) and operation parameters (2.0, 3.0).
                {
                    "oper": "add",
                    "params": [
                        2.0,
                        3.0
                    ]
                }
             */

            // this is object
            writer.WriteStartObject();

            writer.WritePropertyName("oper");
            writer.WriteValue(operation.Operation);

            // value of property "params" is array of items (may be numbers, may be objects, may be mixed)
            writer.WritePropertyName("params");
            writer.WriteStartArray();
            foreach (var item in operation.Items)
            {
                serializer.Serialize(writer, item);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
            {
                double value = Convert.ToDouble(reader.Value);
                return new TreeLeaf(value);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                var jsonObject = JObject.Load(reader);
                var operation = jsonObject["oper"]?.ToString();
                var paramsArray = jsonObject["params"]?.ToArray();

                if (operation != null && paramsArray != null)
                {
                    var treeOperation = new TreeOperation(operation);
                    foreach (var param in paramsArray)
                    {
                        var item = param.ToObject<ITreeItem>(serializer);
                        treeOperation.Items.Add(item!);
                    }
                    return treeOperation;
                }
            }

            throw new JsonSerializationException("Unexpected token or value when parsing TreeItem.");
        }

    }

}

