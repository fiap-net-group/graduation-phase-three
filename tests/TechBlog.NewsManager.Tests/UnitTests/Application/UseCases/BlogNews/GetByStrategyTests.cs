using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;
using TechBlog.NewsManager.Tests.UnitTests.Fixtures;

namespace TechBlog.NewsManager.Tests.UnitTests.Application.UseCases.BlogNews
{
    [Collection(nameof(UnitTestsFixtureCollection))]
    public class GetByStrategyTests
    {
        private readonly UnitTestsFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<GetByStrategyRequest> _validator;
        private readonly IGetBlogNewsStrategy _getBlogNewsStrategy;
        private readonly List<IGetBlogNewsStrategy> _getBlogNewsStrategies;

        public GetByStrategyTests(UnitTestsFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _mapper = Substitute.For<IMapper>();
            _validator = Substitute.For<IValidator<GetByStrategyRequest>>();
            _getBlogNewsStrategy = Substitute.For<IGetBlogNewsStrategy>();
            _getBlogNewsStrategies = new List<IGetBlogNewsStrategy>();
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        public void Action_AllInvalidCases_ShouldThrow(int strategiesCount, bool invalidRequest)
        {
            //Arrange
            _getBlogNewsStrategy.RunAsync(Arg.Any<GetBlogNewsStrategyBody>(), Arg.Any<CancellationToken>()).ThrowsAsync<BusinessException>();

            for (int i = 0; i < strategiesCount; i++)
                _getBlogNewsStrategies.Add(_getBlogNewsStrategy);

            if (invalidRequest)
                _validator.Validate(Arg.Any<GetByStrategyRequest>()).Throws(new ValidationException("Invalid request"));

            //Act
            var act = async () => await GetByStrategyHandler.Action(_logger, _mapper, _validator, _getBlogNewsStrategies, GetBlogNewsStrategy.GET_BY_TAGS, Guid.NewGuid(), null, null, null, null, CancellationToken.None);

            //Assert
            act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task RunAsync_SuccessCaseWithSingleResult_ShouldReturnSuccess()
        {
            //Arrange
            _getBlogNewsStrategy.RunAsync(Arg.Any<GetBlogNewsStrategyBody>(), Arg.Any<CancellationToken>()).Returns(new BlogUserViewModel());
            _getBlogNewsStrategy.Strategy.Returns(GetBlogNewsStrategy.GET_BY_ID);
            _getBlogNewsStrategies.Add(_getBlogNewsStrategy);

            //Act
            var response = await GetByStrategyHandler.Action(_logger, _mapper, _validator, _getBlogNewsStrategies, GetBlogNewsStrategy.GET_BY_ID, Guid.NewGuid(), null, null, null, null, CancellationToken.None);
            var responseContext = HttpContextFixtures.GetResposeHttpContext(response);
            var responseBody = HttpContextFixtures.GetObjectFromBodyAsync<BaseResponseWithValue<BlogNewViewModel>>(responseContext).Result;

            //Assert
            responseContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            responseBody.Should().NotBeNull();
            responseBody.Success.Should().Be(true);
            responseBody.Value.Should().NotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(100000)]
        public async Task RunAsync_SuccessCaseWithCollectionAsResult_ShouldReturnSuccess(int resultQuantity)
        {
            //Arrange
            _getBlogNewsStrategy.RunAsync(Arg.Any<GetBlogNewsStrategyBody>(), Arg.Any<CancellationToken>()).Returns(_fixture.BlogNewViewModelFaker.Generate(resultQuantity));
            _getBlogNewsStrategy.Strategy.Returns(GetBlogNewsStrategy.GET_BY_NAME);
            _getBlogNewsStrategies.Add(_getBlogNewsStrategy);

            //Act
            var response = await GetByStrategyHandler.Action(_logger, _mapper, _validator, _getBlogNewsStrategies, GetBlogNewsStrategy.GET_BY_NAME, null, "name", null, null, null, CancellationToken.None);
            var responseContext = HttpContextFixtures.GetResposeHttpContext(response);
            var responseBody = HttpContextFixtures.GetObjectFromBodyAsync<BaseResponseWithValue<IEnumerable<BlogNewViewModel>>>(responseContext).Result;

            //Assert
            responseContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            responseBody.Should().NotBeNull();
            responseBody.Success.Should().Be(true);
            responseBody.Value.Should().NotBeNull();
            responseBody.Value.Count().Should().Be(resultQuantity);
        }
    }
}
