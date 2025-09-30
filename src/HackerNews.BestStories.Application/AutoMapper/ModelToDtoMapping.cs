using AutoMapper;
using HackerNews.BestStories.Domain;
using HackerNews.BestStories.Shared;

namespace HackerNews.BestStories.Application.AutoMapper;

public class ModelToDtoMappingProfile : Profile
{
    public ModelToDtoMappingProfile()
    {
        CreateMap<HackerNewsItem, HackerNewsItemDto>()
            .ForMember(dto => dto.By, 
                opt => opt.MapFrom(map => map.PostedBy))
            .ForMember(dto => dto.Descendants, 
                opt => opt.MapFrom(map => map.CommentCount))
            .ForMember(dto => dto.Url,
                opt => opt.MapFrom(map => map.Uri))
            .ForMember(dto => dto.Time,
                opt => opt.MapFrom(src => new DateTimeOffset(src.Time).ToUnixTimeSeconds()));
    }
}