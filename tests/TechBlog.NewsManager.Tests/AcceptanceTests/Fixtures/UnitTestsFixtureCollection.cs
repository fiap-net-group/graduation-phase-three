using Bogus;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Entities;

namespace TechBlog.NewsManager.Tests.AcceptanceTests.Fixtures
{
    [CollectionDefinition(nameof(UnitTestsFixtureCollection))]
    public class UnitTestsFixtureCollection : ICollectionFixture<UnitTestsFixture> { }

    public class UnitTestsFixture
    {
        public AuthorizationFixtures Authorization { get; set; }
        public HttpContextFixtures HttpContext { get; set; }

        public Faker<BlogNew> BlogNewFaker { get; init; }
        public Faker<BlogNewViewModel> BlogNewViewModelFaker { get; init; }
        public BlogNewFixtures BlogNew { get; set; }

        public UnitTestsFixture()
        {
            Authorization = new();
            HttpContext = new();
            BlogNewFaker = new();
            BlogNewViewModelFaker = new();
            BlogNew = new();
        }
    }
}
