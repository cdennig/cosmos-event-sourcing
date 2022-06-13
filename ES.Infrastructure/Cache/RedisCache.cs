using ES.Shared.Cache;
using EasyCaching.Core;

namespace ES.Infrastructure.Cache;

public class RedisCache : ICache
{
    private IEasyCachingProviderFactory _factory;
    private readonly IEasyCachingProvider _provider;

    public RedisCache(IEasyCachingProviderFactory factory)
    {
        _factory = factory;
        _provider = _factory.GetCachingProvider("redis");
    }

    public async Task<string> GetItemAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty");
        var response = await _provider.GetAsync<string>(key);
        return response.Value;
    }

    public Task SetItemAsync(string key, string value, TimeSpan expiresIn)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Key and/or value cannot be empty");
        return _provider.SetAsync(key, value, expiresIn);
    }

    public Task InvalidateItemAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty");
        return _provider.RemoveAsync(key);
    }
}