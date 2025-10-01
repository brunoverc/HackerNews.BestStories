using System.Net;
using System.Net.Http.Json;
using HackerNews.BestStories.Infrastructure.Clients;
using HackerNews.BestStories.Shared;
using Moq;
using Moq.Protected;

public class HackerNewsClientTests
{
    [Fact]
    public async Task GetBestStoriesIdsAsync_ReturnsIds()
    {
        var expectedIds = new[] { 1, 2, 3 };
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedIds)
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
        };
        var client = new HackerNewsClient(httpClient);

        var result = await client.GetBestStoriesIdsAsync();

        Assert.Equal(expectedIds, result);
    }

    [Fact]
    public async Task GetStoryByIdAsync_ReturnsStory()
    {
        var expectedStory = new HackerNewsItemDto { Id = 1, Title = "Test Story" };
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedStory)
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
        };
        var client = new HackerNewsClient(httpClient);

        var result = await client.GetStoryByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(expectedStory.Id, result.Id);
        Assert.Equal(expectedStory.Title, result.Title);
    }
}
