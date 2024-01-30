using AutoMapper;
using TechBlog.NewsManager.API.Application.Mapper;

namespace TechBlog.NewsManager.Tests.UnitTests.Application.Mapper
{
    public class StrategyMapperTests
    {
        [Fact]
        public void AutoMapper_CaseConfiguration_IsValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<StrategyMapper>();
            });

            // Act & Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
