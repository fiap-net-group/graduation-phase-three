using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using TechBlog.NewsManager.API.Application.Mapper;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Create;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy.Strategies;
using TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create;
using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;

namespace TechBlog.NewsManager.API.DependencyInjection.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationConfiguration
    {
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BlogUserMapper));
            services.AddAutoMapper(typeof(BlogNewMapper));

            services.AddValidatorsFromAssemblyContaining<CreateBlogUserValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateBlogNewValidator>();

            services.AddStrategies();

            return services;
        }

        private static IServiceCollection AddStrategies(this IServiceCollection services)
        {
            services.AddScoped<IGetBlogNewsStrategy, GetByCreateDateStrategy>();
            services.AddScoped<IGetBlogNewsStrategy, GetByCreateOrUpdateDateStrategy>();
            services.AddScoped<IGetBlogNewsStrategy, GetByIdStrategy>();
            services.AddScoped<IGetBlogNewsStrategy, GetByTagStrategy>();
            services.AddScoped<IGetBlogNewsStrategy, GetByNameStrategy>();

            return services;
        }
    }
}
