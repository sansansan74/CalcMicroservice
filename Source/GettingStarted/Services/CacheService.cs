using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CalcMicroservice.Services;

public class CacheService
{
    private readonly IDistributedCache _cache;
    private const int CacheExpirationMinutes = 5;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string> GetValue(string expression)
    {
        string key = ComputeHash(expression);
        var cachedResult = await _cache.GetStringAsync(key);

        return cachedResult;
    }

    public async Task SetValue(string key, string value)
    {
        string hashedKey = ComputeHash(key);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
        };

        await _cache.SetStringAsync(hashedKey, value, options);
    }

    private static string ComputeHash(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}