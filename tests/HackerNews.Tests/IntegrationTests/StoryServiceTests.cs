using AutoMapper;
using FluentAssertions;
using Xunit;
using Moq;
using HackerNews.BestStories.Application.Services;
using HackerNews.BestStories.Domain;
using HackerNews.BestStories.Infrastructure.Cache;
using HackerNews.BestStories.Infrastructure.Clients.Interfaces;
using HackerNews.BestStories.Infrastructure.RateLimiting.Interfaces;
using HackerNews.BestStories.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace HackerNews.Tests.IntegrationTests;

public class StoryServiceTests
{
    private StoryService CreateService(Mock<IHackerNewsClient> mockClient)
    {
        var config = new MapperConfiguration(cfg => 
            cfg.CreateMap<HackerNewsItemDto, HackerNewsItem>(),
            NullLoggerFactory.Instance
        );

        var mapper = config.CreateMapper();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cacheService = new CacheService(memoryCache);
        
        var mockRateLimiter = new Mock<IExternalRateLimiter>();
        mockRateLimiter
            .Setup(x => x.AcquireAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        return new StoryService(mockClient.Object, mapper, cacheService, mockRateLimiter.Object);
    }

    [Fact]
    public async Task Should_Call_Api_Once_And_Then_Return_From_Cache()
    {
        var storyId = 123;
        var mockClient = new Mock<IHackerNewsClient>();
        mockClient.Setup(x => x.GetStoryByIdAsync(storyId, default))
            .ReturnsAsync(new HackerNewsItemDto
            {
                Title = "Cached Story",
                Score = 100
            });

        var service = CreateService(mockClient);

        var firstCall = await service.GetStoryByIdAsync(storyId);
        var secondCall = await service.GetStoryByIdAsync(storyId);

        firstCall.Should().NotBeNull();
        secondCall.Should().NotBeNull();
        secondCall!.Title.Should().Be("Cached Story");

        mockClient.Verify(x => x.GetStoryByIdAsync(storyId, default), Times.Once);
    }

    [Fact]
    public async Task Should_Expire_Cache_And_Call_Api_Again()
    {
        var storyId = 456;
        var mockClient = new Mock<IHackerNewsClient>();
        mockClient.SetupSequence(x => x.GetStoryByIdAsync(storyId, default))
            .ReturnsAsync(new HackerNewsItemDto { Title = "First Call", Score = 50 })
            .ReturnsAsync(new HackerNewsItemDto { Title = "Second Call", Score = 80 });

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cacheService = new CacheService(memoryCache);

        var config = new MapperConfiguration(
            cfg => cfg.CreateMap<HackerNewsItemDto, HackerNewsItem>(),
            NullLoggerFactory.Instance);
        var mapper = config.CreateMapper();
        
        var mockRateLimiter = new Mock<IExternalRateLimiter>();
        mockRateLimiter
            .Setup(x => x.AcquireAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new StoryService(mockClient.Object, mapper, cacheService, mockRateLimiter.Object);

        var first = await service.GetStoryByIdAsync(storyId);

        memoryCache.Remove($"story_{storyId}");

        var second = await service.GetStoryByIdAsync(storyId);

        first!.Title.Should().Be("First Call");
        second!.Title.Should().Be("Second Call");

        mockClient.Verify(x => x.GetStoryByIdAsync(storyId, default), Times.Exactly(2));
    }

    [Fact]
    public async Task Should_Cache_Different_Stories_Separately()
    {
        var mockClient = new Mock<IHackerNewsClient>();
        mockClient.Setup(x => x.GetStoryByIdAsync(1, default))
            .ReturnsAsync(new HackerNewsItemDto { Title = "Story 1" });
        mockClient.Setup(x => x.GetStoryByIdAsync(2, default))
            .ReturnsAsync(new HackerNewsItemDto { Title = "Story 2" });

        var service = CreateService(mockClient);

        var first = await service.GetStoryByIdAsync(1);
        var second = await service.GetStoryByIdAsync(2);
        var again = await service.GetStoryByIdAsync(1);

        first!.Title.Should().Be("Story 1");
        second!.Title.Should().Be("Story 2");
        again!.Title.Should().Be("Story 1");

        mockClient.Verify(x => x.GetStoryByIdAsync(1, default), Times.Once);
        mockClient.Verify(x => x.GetStoryByIdAsync(2, default), Times.Once);
    }
}
