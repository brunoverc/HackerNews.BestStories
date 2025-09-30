using System.Net.Http.Json;
using HackerNews.BestStories.Infrastructure.Clients.Interfaces;
using HackerNews.BestStories.Shared;

namespace HackerNews.BestStories.Infrastructure.Clients;

public class HackerNewsClient : IHackerNewsClient
{
    private readonly HttpClient _httpClient;

    public HackerNewsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
    }

    public async Task<IEnumerable<int>>? GetBestStoriesIdsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<IEnumerable<int>>(
            "beststories.json", cancellationToken);
        
        return result ?? Enumerable.Empty<int>();
    }

    public async Task<HackerNewsItemDto?> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<HackerNewsItemDto>(
            $"item/{id}.json", cancellationToken);
    }
}