using HackerNews.BestStories.Api.Middleware;
using HackerNews.BestStories.Application.AutoMapper;

namespace HackerNews.BestStories.Api.Configuration;

public static class ApiConfig
{
    public static void ConfigureStartupConfiguration(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
    
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();
        builder.Services.AddRateLimiting();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerConfiguration();
        builder.Services.RegisterDependencies();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperConfig));
        builder.Services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

        builder.Services.AddHealthChecks()
            .AddUrlGroup(new Uri("https://hacker-news.firebaseio.com/v0/beststories.json"), name: "hackernews-api")
            .AddPrivateMemoryHealthCheck(maximumMemoryBytes: 512 * 1024 * 1024, name: "private-memory");

    }
    
    public static WebApplication UseStartupConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseRateLimiter();

        if (!app.Environment.IsProduction())
            app.UseDeveloperExceptionPage();

        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseRouting();
        }
        
        app.UseMiddleware<ErrorHandlerMiddleware>();
        
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                });
                await context.Response.WriteAsync(result);
            }
        }).DisableRateLimiting();

        app.MapControllers();

        return app;
    }
}