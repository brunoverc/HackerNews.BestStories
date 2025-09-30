using HackerNews.BestStories.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// add serilog
//builder.Host.ConfigureSeriLog();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

// Add services to the container.
builder.ConfigureStartupConfiguration();

builder.Build().UseStartupConfiguration().Run();

public partial class Program { }