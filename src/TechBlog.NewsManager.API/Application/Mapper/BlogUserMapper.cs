using AutoMapper;
using TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.API.Application.Mapper
{
    public class BlogUserMapper : Profile
    {
        public BlogUserMapper()
        {
            CreateMap<CreateBlogUserRequest, BlogUser>()
                .ForMember(e => e.WrittenNews, mapper => mapper.Ignore())
                .ForMember(e => e.Enabled, mapper => mapper.MapFrom(r => true))
                .ForMember(e => e.CreatedAt, mapper => mapper.MapFrom(r => DateTime.Now))
                .ForMember(e => e.LastUpdateAt, mapper => mapper.MapFrom(r => DateTime.Now))
                .ForMember(e => e.UserName, mapper => mapper.MapFrom(r => r.Email))
                .ForMember(e => e.NormalizedUserName, mapper => mapper.MapFrom(r => r.Email.ToLower()))
                .ForMember(e => e.NormalizedEmail, mapper => mapper.MapFrom(r => r.Email.ToLower()))
                .ForMember(e => e.Id, mapper => mapper.MapFrom(r => Guid.NewGuid().ToString()))
                .ForMember(e => e.PasswordHash, mapper => mapper.Ignore())
                .ForMember(e => e.SecurityStamp, mapper => mapper.Ignore())
                .ForMember(e => e.ConcurrencyStamp, mapper => mapper.Ignore())
                .ForMember(e => e.PhoneNumber, mapper => mapper.Ignore())
                .ForMember(e => e.PhoneNumberConfirmed, mapper => mapper.Ignore())
                .ForMember(e => e.TwoFactorEnabled, mapper => mapper.MapFrom(r => false))
                .ForMember(e => e.LockoutEnabled, mapper => mapper.MapFrom(r => false))
                .ForMember(e => e.LockoutEnd, mapper => mapper.Ignore())
                .ForMember(e => e.AccessFailedCount, mapper => mapper.Ignore())
                .ForMember(e => e.EmailConfirmed, mapper => mapper.MapFrom(r => true))
                .ForMember(e => e.InternalId, mapper => mapper.Ignore());

            CreateMap<BlogUser, BlogUserViewModel>().ReverseMap()
                .ForMember(e => e.InternalId, mapper => mapper.Ignore())
                .ForMember(e => e.WrittenNews, mapper => mapper.Ignore())
                .ForMember(e => e.Enabled, mapper => mapper.Ignore())
                .ForMember(e => e.CreatedAt, mapper => mapper.Ignore())
                .ForMember(e => e.LastUpdateAt, mapper => mapper.Ignore())
                .ForMember(e => e.UserName, mapper => mapper.Ignore())
                .ForMember(e => e.NormalizedUserName, mapper => mapper.Ignore())
                .ForMember(e => e.NormalizedEmail, mapper => mapper.Ignore())
                .ForMember(e => e.EmailConfirmed, mapper => mapper.Ignore())
                .ForMember(e => e.PasswordHash, mapper => mapper.Ignore())
                .ForMember(e => e.SecurityStamp, mapper => mapper.Ignore())
                .ForMember(e => e.ConcurrencyStamp, mapper => mapper.Ignore())
                .ForMember(e => e.PhoneNumber, mapper => mapper.Ignore())
                .ForMember(e => e.PhoneNumberConfirmed, mapper => mapper.Ignore())
                .ForMember(e => e.TwoFactorEnabled, mapper => mapper.Ignore())
                .ForMember(e => e.LockoutEnd, mapper => mapper.Ignore())
                .ForMember(e => e.LockoutEnabled, mapper => mapper.Ignore())
                .ForMember(e => e.AccessFailedCount, mapper => mapper.Ignore());
        }
    }
}