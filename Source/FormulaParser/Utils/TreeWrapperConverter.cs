using FormulaParser.Tree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MathExpressionParser.Utils
{
    /// <summary>
    /// Class convert to JSon wrapper of tree expression: TreeWrapper
    /// </summary>
    class TreeWrapperConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(TreeWrapper).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is TreeWrapper wrapper)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("value");
                serializer.Serialize(writer, wrapper.Value);

                writer.WriteEndObject();
                return;
            }

            throw new InvalidOperationException("Unknown TreeWrapper type");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var valueToken = jsonObject["value"];
            if (valueToken != null)
            {
                var value = valueToken.ToObject<ITreeItem>(serializer);
                return new TreeWrapper(value!);
            }

            throw new JsonSerializationException("Unexpected token or value when parsing TreeWrapper.");
        }
    }

}

