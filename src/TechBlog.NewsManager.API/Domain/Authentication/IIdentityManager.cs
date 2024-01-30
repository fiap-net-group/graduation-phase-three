using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.API.Domain.Authentication
{
    public interface IIdentityManager
    {
        Task<BlogUser> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);
        Task<bool> CreateUserAsync(BlogUser user, string password, CancellationToken cancellationToken);
        Task<AccessTokenModel> AuthenticateAsync(BlogUser user, string password, CancellationToken cancellationToken, params (string name, string value)[] customClaims);
    }
}
