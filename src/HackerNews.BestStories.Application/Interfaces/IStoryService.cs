using HackerNews.BestStories.Domain;

namespace HackerNews.BestStories.Application.Interfaces;

public interface IStoryService
{
    Task<IEnumerable<int>> GetStoryIdsAsync(CancellationToken cancellationToken = default);
    Task<HackerNewsItem?> GetStoryByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<HackerNewsItem>> GetBestStoriesAsync(int numberOfStories,
        CancellationToken cancellationToken = default);


}