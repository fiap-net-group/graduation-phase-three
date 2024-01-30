namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.Update
{
    public class UpdateBlogNewRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
    }
}