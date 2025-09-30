using HackerNews.BestStories.Shared;

namespace HackerNews.BestStories.Infrastructure.Clients.Interfaces;

public interface IHackerNewsClient
{
    Task<IEnumerable<int>> GetBestStoriesIdsAsync (CancellationToken cancellationToken = default);
    Task<HackerNewsItemDto?> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default); 
}