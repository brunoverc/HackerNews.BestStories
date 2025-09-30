using AutoMapper;
using HackerNews.BestStories.Domain;
using HackerNews.BestStories.Shared;

namespace HackerNews.BestStories.Application.AutoMapper;

public class DtoToModelMappingProfile : Profile
{
    public DtoToModelMappingProfile()
    {
        CreateMap<HackerNewsItemDto, HackerNewsItem>()
            .ForMember(dto => dto.PostedBy, 
                opt => opt.MapFrom(map => map.By))
            .ForMember(dto => dto.CommentCount, 
                opt => opt.MapFrom(map => map.Descendants))
            .ForMember(dto => dto.Uri,
                        opt => opt.MapFrom(map => map.Url))
            .ForMember(dto => dto.Time,
                opt => opt.MapFrom(map => DateTimeOffset.FromUnixTimeSeconds(map.Time).UtcDateTime));
    }
}