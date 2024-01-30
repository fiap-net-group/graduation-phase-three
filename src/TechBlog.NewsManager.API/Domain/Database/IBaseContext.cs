namespace TechBlog.NewsManager.API.Domain.Database
{
    public interface IBaseContext : IDisposable
    {
        Task<bool> AnyPendingMigrationsAsync();
        Task MigrateAsync();
        Task TestConnectionAsync();
    }
}
