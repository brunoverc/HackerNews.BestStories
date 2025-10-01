using AutoMapper;
using HackerNews.BestStories.Application.Interfaces;
using HackerNews.BestStories.Domain;
using HackerNews.BestStories.Infrastructure.Cache.Interfaces;
using HackerNews.BestStories.Infrastructure.Clients.Interfaces;
using HackerNews.BestStories.Infrastructure.RateLimiting.Interfaces;
using HackerNews.BestStories.Shared;
using Microsoft.AspNetCore.Identity;

namespace HackerNews.BestStories.Application.Services;

public class StoryService : IStoryService
{
    private readonly IHackerNewsClient _hackerNewsClient;
    private readonly IMapper _map;
    private readonly ICacheService _cacheService;
    private readonly IExternalRateLimiter _rateLimiter;
    
    private const string BestStoriesCacheKey = "beststories_ids";

    public StoryService(IHackerNewsClient hackerNewsClient,
        IMapper mapper,
        ICacheService cacheService,
        IExternalRateLimiter rateLimiter)
    {
        _hackerNewsClient = hackerNewsClient;
        _map = mapper;
        _cacheService = cacheService;
        _rateLimiter = rateLimiter;
    }

    public async Task<IEnumerable<int>> GetStoryIdsAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cacheService.GetAsync<IEnumerable<int>>(BestStoriesCacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }
        
        await ProtectExternalAPI(cancellationToken);
        
        var bestStoriesIds = await _hackerNewsClient.GetBestStoriesIdsAsync(cancellationToken);
        
        await _cacheService.SetAsync(BestStoriesCacheKey, bestStoriesIds, TimeSpan.FromMinutes(5), cancellationToken);
        
        return bestStoriesIds;
    }

    private async Task ProtectExternalAPI(CancellationToken cancellationToken)
    {
        if (!await _rateLimiter.AcquireAsync(cancellationToken))
        {
            throw new InvalidOperationException("Rate limit exceeded for Hacker News API.");
        }
    }

    public async Task<HackerNewsItem?> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = $"story_{id}";
        var cached = await _cacheService.GetAsync<HackerNewsItemDto>(key, cancellationToken);
        if (cached != null)
        {
            return _map.Map<HackerNewsItem>(cached);
        }
        
        await ProtectExternalAPI(cancellationToken);

        
        var storyIn = await _hackerNewsClient.GetStoryByIdAsync(id, cancellationToken);

        if (storyIn != null)
        {
            await _cacheService.SetAsync(key, storyIn, TimeSpan.FromMinutes(10), cancellationToken);
        }

        return _map.Map<HackerNewsItem>(storyIn);
    }

    public async Task<IEnumerable<HackerNewsItem>> GetBestStoriesAsync(int numberOfStories,
        CancellationToken cancellationToken = default)
    {
        var storyIds = await GetStoryIdsAsync(cancellationToken);
        List<HackerNewsItem> stories = new List<HackerNewsItem>();

        foreach (var id in storyIds)
        {
            var story = await GetStoryByIdAsync(id, cancellationToken);
            stories.Add(story);
        }
        
        return stories.OrderByDescending(x => x.Score).Take(numberOfStories);
        
    }
    
    
}