using TechBlog.NewsManager.API.Domain.ValueObjects;

namespace TechBlog.NewsManager.API.Application.ViewModels
{
    public class BlogUserViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public BlogUserType BlogUserType { get; set; }
    }
}
