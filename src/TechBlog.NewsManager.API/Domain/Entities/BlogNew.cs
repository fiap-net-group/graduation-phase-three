namespace TechBlog.NewsManager.API.Domain.Entities
{
    public sealed class BlogNew
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string InternalTags { get; set; }
        public string[] Tags
        {
            get
            {
                return string.IsNullOrWhiteSpace(InternalTags) ? Array.Empty<string>() : InternalTags.Split(';');
            }
            set
            {
                InternalTags = value.Length > 0 ? string.Join(";", value) : string.Empty;
            }
        }
        public bool Enabled { get; set; }
        public string AuthorId { get; set; }
        public BlogUser Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }

        public void Update(string title, string description, string body)
        {
            Title = title;
            Description = description;
            Body = body;
            LastUpdateAt = DateTime.UtcNow;
        }
    }
}
