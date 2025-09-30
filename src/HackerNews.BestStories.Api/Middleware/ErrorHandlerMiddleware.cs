using System.Net;
using System.Text.Json;
using HackerNews.BestStories.Shared.Domain;
using Serilog;

namespace HackerNews.BestStories.Api.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            Log.Error(error, "An unexpected error occurred");
            var response = context.Response;
            response.ContentType = "application/json";
            switch (error)
            {
                case DomainException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            
            var result = JsonSerializer.Serialize(new { message = error.Message });
            
            await response.WriteAsync(result);
        }
    }
}