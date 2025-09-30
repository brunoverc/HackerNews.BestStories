using System.Threading.RateLimiting;
using HackerNews.BestStories.Api.Middleware;
using HackerNews.BestStories.Application.AutoMapper;
using Microsoft.AspNetCore.RateLimiting;

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
    }
    
    public static WebApplication UseStartupConfiguration(this WebApplication app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
            .DisableRateLimiting();
        
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseRateLimiter();

        
        app.MapGet("/api/v1/stories", (int n) => Results.Ok(new { n }))
            .RequireRateLimiting("per-ip");

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

        //app.UseAuthentication();
        //app.UseAuthorization();
        //TODO: app.UseMiddleware<ErrorHandlerMiddleware>();

        app.MapControllers();

        return app;
    }
}