using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.API.Domain.Database
{
    public interface IBlogNewsRepository
    {
        Task AddAsync(BlogNew blogNew, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlogNew>> GetByCreatedDateAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlogNew>> GetByCreateOrUpdateDateAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<BlogNew> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlogNew>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlogNew>> GetByTagsAsync(string[] tags, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateAsync(BlogNew blogNew, CancellationToken cancellationToken = default);
    }
}