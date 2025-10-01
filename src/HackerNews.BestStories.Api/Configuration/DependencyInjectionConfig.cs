using HackerNews.BestStories.Infrastructure.Cache;
using HackerNews.BestStories.Infrastructure.Cache.Interfaces;
using HackerNews.BestStories.Infrastructure.Clients;
using HackerNews.BestStories.Infrastructure.Clients.Interfaces;
using HackerNews.BestStories.Infrastructure.RateLimiting;
using HackerNews.BestStories.Infrastructure.RateLimiting.Interfaces;
using Polly;
using Polly.Extensions.Http;

namespace HackerNews.BestStories.Api.Configuration;

using HackerNews.BestStories.Application.AutoMapper;
using Application.Interfaces;
using Application.Services;

public static class DependencyInjectionConfig
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AutoMapperConfig>());

        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
        
        services.AddSingleton<IExternalRateLimiter, HackerNewsRateLimiter>();
        
        services.AddScoped<IHackerNewsClient, HackerNewsClient>();
        services.AddScoped<IStoryService, StoryService>();

        services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client =>
            {
                client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError() // 5xx, 408, network errors
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // também cobre 429
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt =>
                        TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)), // 200ms, 400ms, 800ms
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine(
                            $"Retry {retryAttempt} after {timespan.TotalMilliseconds}ms due to {outcome.Result?.StatusCode}");
                    }))
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, timespan) =>
                    {
                        Console.WriteLine(
                            $"Circuit opened for {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                    },
                    onReset: () => Console.WriteLine("Circuit closed, operations normal."),
                    onHalfOpen: () => Console.WriteLine("Circuit half-open, next call is a trial.")));
    }
}