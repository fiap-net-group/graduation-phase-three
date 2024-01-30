namespace TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews
{
    public class GetBlogNewsStrategyBody
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public bool ValidId => Id != Guid.Empty;
        public bool ValidName => !string.IsNullOrWhiteSpace(Name);
        public bool ValidTags => Tags != null && Tags.Length > 0;
        public bool ValidDateInterval => To != DateTime.MinValue &&
                                         From != DateTime.MinValue &&
                                         From <= To;

        public GetBlogNewsStrategyBody() { }

        public GetBlogNewsStrategyBody(Guid id)
        {
            Id = id;
        }

        public GetBlogNewsStrategyBody(string name)
        {
            Name = name;
        }

        public GetBlogNewsStrategyBody(string[] tags)
        {
            Tags = tags;
        }

        public GetBlogNewsStrategyBody(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
    }
}
