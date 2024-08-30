using FormulaParser.Tree;
using Newtonsoft.Json;

namespace FormulaCalculator.Utils
{
    public static class CalcExpressionSerializer
    {

        public static string SerializeToJson(CalcTreeOperation item)
        {
            return JsonConvert.SerializeObject(item, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                Converters = GetConverters()
            });
        }
        public static CalcTreeOperation DeserializeFromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = GetConverters()
            };

            var operation = JsonConvert.DeserializeObject<CalcTreeOperation>(json, settings);
            return operation!;
        }

        private static List<JsonConverter> GetConverters() => new List<JsonConverter> {
                    new CalcTreeItemConverter()
                };
    }

}

