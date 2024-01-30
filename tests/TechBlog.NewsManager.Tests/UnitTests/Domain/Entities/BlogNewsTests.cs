using FluentAssertions;
using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.Tests.UnitTests.Domain.Entities
{
    public class BlogNewsTests
    {
        [Theory]
        [InlineData("fake;tag", "fake", "tag")]
        [InlineData("fake;tag;more", "fake", "tag", "more")]
        [InlineData("")]
        public void Tags_ShouldMatchExpected(string expetedInternalTags, params string[] tags)
        {
            //Arrange
            var blogNew = new BlogNew
            {
                //Act
                Tags = tags
            };

            //Assert
            for (int i = 0; i < tags.Length; i++)
                blogNew.Tags[i].Should().Be(tags[i]);

            blogNew.InternalTags.Should().Be(expetedInternalTags);
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData(null, null, null)]
        [InlineData("tittle", "desc", "body")]
        public void Update_ShouldMatchExpedted(string title, string description, string body)
        {
            //Arrange
            var id = Guid.NewGuid();
            var initialCreatedAndUpdatedAt = DateTime.Now;
            var blogNew = new BlogNew
            {
                Id = id,
                Title = "initial tittle",
                Description = "initial desc",
                Body = "initial body",
                CreatedAt = initialCreatedAndUpdatedAt,
                LastUpdateAt = initialCreatedAndUpdatedAt
            };

            //Act
            blogNew.Update(title, description, body);

            //Assert
            blogNew.Id.Should().Be(id);
            blogNew.Title.Should().Be(title);
            blogNew.Description.Should().Be(description);
            blogNew.Body.Should().Be(body);
            blogNew.CreatedAt.Should().Be(initialCreatedAndUpdatedAt);
            blogNew.LastUpdateAt.Should().NotBe(initialCreatedAndUpdatedAt);
        }
    }
}
