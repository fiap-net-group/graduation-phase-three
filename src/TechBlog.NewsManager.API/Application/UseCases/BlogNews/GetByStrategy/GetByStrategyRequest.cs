using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy
{
    public class GetByStrategyRequest
    {
        public GetBlogNewsStrategy Strategy { get; set; }
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public GetByStrategyRequest(GetBlogNewsStrategy strategy,
                                    Guid? id,
                                    string name,
                                    string[] tags,
                                    DateTime? from,
                                    DateTime? to)
        {
            Strategy = strategy;
            Id = id;
            Name = name;
            Tags = tags;
            From = from;
            To = to;
        }
    }
}
