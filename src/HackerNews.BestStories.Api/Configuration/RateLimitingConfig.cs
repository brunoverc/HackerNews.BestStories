using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace HackerNews.BestStories.Api.Configuration;

public static class RateLimitingConfig
{
    public static void AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(policyName: "fixed", configure =>
            {
                configure.PermitLimit = 100;
                configure.Window = TimeSpan.FromMinutes(1);
                configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                configure.QueueLimit = 0;
            });

            options.AddTokenBucketLimiter(policyName: "token", configure =>
            {
                configure.TokenLimit = 50;
                configure.QueueLimit = 0;
                configure.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
                configure.TokensPerPeriod = 1;
                configure.AutoReplenishment = true;
            });

            options.AddConcurrencyLimiter(policyName: "concurrent", configure =>
            {
                configure.PermitLimit = 20;
                configure.QueueLimit = 0;
                configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.AddSlidingWindowLimiter(policyName: "sliding", configure =>
            {
                configure.PermitLimit = 200;
                configure.Window = TimeSpan.FromMinutes(1);
                configure.SegmentsPerWindow = 4;
                configure.QueueLimit = 0;
                configure.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.AddPolicy(policyName: "per-ip", partitioner: httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            options.AddPolicy(policyName: "per-user", partitioner: httpContext =>
            {
                var userId = httpContext.User?.Identity?.IsAuthenticated == true
                    ? httpContext.User.Identity!.Name!
                    : httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon";
                return RateLimitPartition.GetTokenBucketLimiter(userId, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    TokensPerPeriod = 1,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                    AutoReplenishment = true,
                    QueueLimit = 0
                });
            });

            options.AddPolicy(policyName: "per-api-key", partitioner: httpContext =>
            {
                var key = httpContext.Request.Headers["X-Api-Key"].FirstOrDefault()
                          ?? httpContext.Connection.RemoteIpAddress?.ToString()
                          ?? "unknown";
                return RateLimitPartition.GetSlidingWindowLimiter(key, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 300,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
            });

            options.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                await context.HttpContext.Response.WriteAsync(
                    $$"""
                      {
                        "error": "rate_limited",
                        "message": "Too many requests. Please try again later."
                      }
                      """, token);
            };

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 300,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
    }
}
