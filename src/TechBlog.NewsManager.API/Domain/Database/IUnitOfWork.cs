namespace TechBlog.NewsManager.API.Domain.Database
{
    public interface IUnitOfWork : IDisposable
    {
        IBlogNewsRepository BlogNew { get; }

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task<bool> CommitTransactionAsync(CancellationToken cancellationToken);
    }
}