namespace HackerNews.BestStories.Infrastructure.Cache.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);
    
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default);
}