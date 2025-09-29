using Microsoft.AspNetCore.Mvc;

namespace HackerNews.BestStories.Api.V1.Controllers;

public class StoriesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}