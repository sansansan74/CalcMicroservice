using FormulaCalculator.Utils;
using FormulaParser.Tree;
using FormulaParser.Utils;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CalcMicroservice.Services;

public class TreePersistantStorageService(CacheService cache, IConfiguration configuration) : ITreePersistantStorageService
{
    private readonly CacheService _cache = cache;
    private readonly int _cacheExpirationSeconds = configuration.GetValue<int>("Redis:cache_duration_seconds", 300);

    public async Task Set(ICalcTreeItem calcTree, string value)
    {
        var json = CalcExpressionSerializer.SerializeToJson(calcTree as CalcTreeOperation);
        await _cache.SetValue(json, value);
    }

    public async Task Set(string calcTree, string value)
    {
        await _cache.SetValue(calcTree, value);
    }

    public async Task<string> Get(ICalcTreeItem calcTree)
    {
        ITreeItem tree = CalcTreeUtils.CreateTree(calcTree);
        var json = TreePrinter.Print(tree);

        var cachedResult = await _cache.GetValue(json);

        return cachedResult;
    }

    public async Task<string> Get(string calcTree)
    {
        var cachedResult = await _cache.GetValue(calcTree);

        return cachedResult;
    }
}
