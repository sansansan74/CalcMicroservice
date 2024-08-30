using FormulaParser.Tree;
using MathExpressionParser.Utils;
using Newtonsoft.Json;

namespace FormulaParser.Utils
{
    public static class ExpressionSerializer
    {
        //public static string SerializeToJson(ITreeItem item)
        //{
        //    return JsonConvert.SerializeObject(item, Formatting.Indented, new JsonSerializerSettings
        //    {
        //        TypeNameHandling = TypeNameHandling.None,
        //        Converters = new List<JsonConverter> { new TreeItemConverter() }
        //    });
        //}

        public static string SerializeToJson(TreeWrapper item)
        {
            return JsonConvert.SerializeObject(item, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                Converters = GetConverters()
            });
        }



        public static TreeWrapper DeserializeFromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = GetConverters()
            };

            TreeWrapper wrapper = JsonConvert.DeserializeObject<TreeWrapper>(json, settings)!;
            return wrapper;
        }

        private static List<JsonConverter> GetConverters() => new List<JsonConverter> {
                    new TreeWrapperConverter(),
                    new TreeItemConverter()
                };
    }

}

