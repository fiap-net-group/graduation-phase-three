namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.Create
{
    public class CreateBlogNewRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        public bool Enabled { get; set; }
    }
}