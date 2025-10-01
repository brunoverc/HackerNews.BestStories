namespace HackerNews.BestStories.Infrastructure.RateLimiting.Interfaces;

public interface IExternalRateLimiter
{
    Task<bool> AcquireAsync(CancellationToken cancellationToken = default);
}