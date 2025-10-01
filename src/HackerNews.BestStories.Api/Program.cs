using HackerNews.BestStories.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.ConfigureStartupConfiguration();

builder.Build().UseStartupConfiguration().Run();

public partial class Program { }