namespace TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews
{
    public interface IGetBlogNewsStrategy
    {
        GetBlogNewsStrategy Strategy { get; }
        Task<object> RunAsync(GetBlogNewsStrategyBody body, CancellationToken cancellationToken);
    }
}
