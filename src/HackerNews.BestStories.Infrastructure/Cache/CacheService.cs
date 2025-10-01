using Microsoft.Extensions.Caching.Memory;

using HackerNews.BestStories.Infrastructure.Cache.Interfaces;

namespace HackerNews.BestStories.Infrastructure.Cache;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken token = default)
    {
        _memoryCache.Set(key, value, ttl);
        return Task.CompletedTask;
    }
}