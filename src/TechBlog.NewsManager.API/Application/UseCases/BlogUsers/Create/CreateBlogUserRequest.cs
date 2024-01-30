using TechBlog.NewsManager.API.Domain.ValueObjects;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create
{
    public sealed class CreateBlogUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public BlogUserType BlogUserType { get; set; }

        public CreateBlogUserRequest() { }

        public CreateBlogUserRequest(string email, string password, string name, BlogUserType blogUserType)
        {
            Email = email;
            Password = password;
            Name = name;
            BlogUserType = blogUserType;
        }
    }
}
