using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace HackerNews.BestStories.Api.Configuration
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<SwaggerExcludePropertiesFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Hacker news best stories",
                    Description = "This API RESTful API to retrieve the details of the best n stories from the Hacker News API",
                    Contact = new OpenApiContact() { Name = "Bruno Verçosa", Email = "bruno.nv@hotmail.com" }
                });

            });
        }

        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        public class SwaggerExcludePropertiesFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                var excludeProperties = new[]
                {
                    "notifications", "isValid", "valid", "invalid", "validationResult", "ruleSetsExecuted", "ValidationResult"
                };

                foreach (var prop in excludeProperties)
                {
                    if (schema.Properties == null) continue;
                    if (schema.Properties.ContainsKey(prop))
                    {
                        schema.Properties.Remove(prop);
                    }
                }
            }
        }
    }
}
