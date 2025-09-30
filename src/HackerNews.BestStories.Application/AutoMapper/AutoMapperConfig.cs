using AutoMapper;
using Microsoft.Extensions.Logging;

namespace HackerNews.BestStories.Application.AutoMapper;

public class AutoMapperConfig
{
    public static MapperConfiguration RegisterMappings(ILoggerFactory loggerFactory)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DtoToModelMappingProfile>();
            cfg.AddProfile<ModelToDtoMappingProfile>();
        }, loggerFactory);

        return config;
    }
}