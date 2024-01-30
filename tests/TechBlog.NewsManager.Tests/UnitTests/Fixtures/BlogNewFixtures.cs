using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Create;
using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.Tests.UnitTests.Fixtures
{
    public static class BlogNewFixtures
    {
        public static BlogNew GenerateBlogNew(CreateBlogNewRequest request)
        {
            return new BlogNew
            {
                Title = request.Title,
                Description = request.Description,
                Body = request.Body,
                Tags = request.Tags,
                Enabled = request.Enabled,
                CreatedAt = DateTime.Now,
            };
        }
    }
}