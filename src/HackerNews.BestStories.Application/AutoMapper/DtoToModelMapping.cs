using AutoMapper;
using HackerNews.BestStories.Domain;
using HackerNews.BestStories.Shared;

namespace HackerNews.BestStories.Application.AutoMapper;

public class ModelToDtoMappingProfile : Profile
{
    public ModelToDtoMappingProfile()
    {
        CreateMap<HackerNewsItemDto, HackerNewsItem>()
            .ForMember(hni => hni.PostedBy, 
                opt => opt.MapFrom(map => map.By))
            .ForMember(hni => hni.CommentCount, 
                opt => opt.MapFrom(map => map.Descendants))
            .ForMember(hni => hni.Time,
                opt => opt.MapFrom(map => DateTimeOffset.FromUnixTimeSeconds(map.Time).UtcDateTime))
            
            .ForMember(dto => dto.Time,
                opt => opt.MapFrom(map => DateTimeOffset.FromUnixTimeSeconds(map.Time).UtcDateTime));
    }
}