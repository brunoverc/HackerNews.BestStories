using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace IntegrationTests;

public class StoriesEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StoriesEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStories_ShouldReturnOk_AndStoriesList()
    {
        var response = await _client.GetAsync("api/v1/stories/bests/5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var stories = await response.Content.ReadFromJsonAsync<List<StoryResponseDto>>();
        stories.Should().NotBeNull();
        stories.Should().HaveCountLessThanOrEqualTo(5);
        stories!.All(s => !string.IsNullOrEmpty(s.Title)).Should().BeTrue();
    }
}

public class StoryResponseDto
{
    public string Title { get; set; } = default!;
    public string Uri { get; set; } = default!;
    public string PostedBy { get; set; } = default!;
    public DateTime Time { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
}