using System.Threading.RateLimiting;
using HackerNews.BestStories.Infrastructure.RateLimiting.Interfaces;

namespace HackerNews.BestStories.Infrastructure.RateLimiting;

public class HackerNewsRateLimiter : IExternalRateLimiter
{
    private readonly PartitionedRateLimiter<string> _limiter;

    public HackerNewsRateLimiter()
    {
        _limiter = PartitionedRateLimiter.Create<string, string>(partitionKey =>
            RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(1),
                QueueLimit = 20,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));
    }

    public async Task<bool> AcquireAsync(CancellationToken cancellationToken = default)
    {
        using var lease = await _limiter.AcquireAsync("harckernews", 1, cancellationToken);
        return lease.IsAcquired;
    }
}