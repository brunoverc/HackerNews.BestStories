using HackerNews.BestStories.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.BestStories.Api.V1.Controllers;

[Route("api/v1/stories")]
public class StoriesController : Controller
{
    private readonly IStoryService _storyService;

    public StoriesController(IStoryService storyService)
    {
        _storyService = storyService;
    }


    [HttpGet("bests/{amount}")]
    public async Task<IActionResult> GetBestStories(int amount, CancellationToken cancellationToken = default)
    {
        var stories = await _storyService.GetBestStoriesAsync(amount, cancellationToken);
        return stories == null ? NotFound() : Ok(stories);
    }

}