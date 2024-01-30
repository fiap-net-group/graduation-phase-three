using AutoMapper;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy;
using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;

namespace TechBlog.NewsManager.API.Application.Mapper
{
    public class StrategyMapper : Profile
    {
        public StrategyMapper()
        {
            CreateMap<GetByStrategyRequest, GetBlogNewsStrategyBody>()
                .ForMember(s => s.Id, mapper => mapper.MapFrom(r => r.Id ?? Guid.Empty))
                .ForMember(s => s.Name, mapper => mapper.MapFrom(r => r.Name))
                .ForMember(s => s.Tags, mapper => mapper.MapFrom(r => r.Tags))
                .ForMember(s => s.From, mapper => mapper.MapFrom(r => r.From ?? DateTime.MinValue))
                .ForMember(s => s.To, mapper => mapper.MapFrom(r => r.To ?? DateTime.MinValue));
        }
    }
}
