using HackerNews.BestStories.Infrastructure.Cache;
using HackerNews.BestStories.Infrastructure.Cache.Interfaces;
using HackerNews.BestStories.Infrastructure.Clients;
using HackerNews.BestStories.Infrastructure.Clients.Interfaces;

namespace HackerNews.BestStories.Api.Configuration;

using HackerNews.BestStories.Application.AutoMapper;
using HackerNews.BestStories.Application.Interfaces;
using HackerNews.BestStories.Application.Services;

public static class DependencyInjectionConfig
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AutoMapperConfig>());

        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
        
        services.AddScoped<IHackerNewsClient, HackerNewsClient>();
        services.AddScoped<IStoryService, StoryService>();
        
        services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client =>
        {
            client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
        });
        
        
    }

}