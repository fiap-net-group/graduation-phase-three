using AutoMapper;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Create;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.API.Application.Mapper
{
    public class BlogNewMapper : Profile
    {
        public BlogNewMapper()
        {
            CreateMap<CreateBlogNewRequest, BlogNew>();
            CreateMap<BlogNewViewModel, BlogNew>().ReverseMap();
        }
    }
}