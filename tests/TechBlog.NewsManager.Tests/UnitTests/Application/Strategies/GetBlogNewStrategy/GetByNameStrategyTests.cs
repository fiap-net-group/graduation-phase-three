using AutoMapper;
using FluentAssertions;
using NSubstitute;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy.Strategies;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Database;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Strategies.GetBlogNews;
using TechBlog.NewsManager.Tests.UnitTests.Fixtures;

namespace TechBlog.NewsManager.Tests.UnitTests.Application.Strategies.GetBlogNewStrategy
{
    [Collection(nameof(UnitTestsFixtureCollection))]
    public class GetByNameStrategyTests
    {
        private readonly UnitTestsFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IBlogNewsRepository _blogNewsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetByNameStrategyTests(UnitTestsFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _blogNewsRepository = Substitute.For<IBlogNewsRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.BlogNew.Returns(_blogNewsRepository);
            _mapper = Substitute.For<IMapper>();
        }

        [Fact]
        public void Strategy_ShouldMatchDefined()
        {
            //Arrange
            var sut = new GetByNameStrategy(_logger, _unitOfWork, _mapper);

            //Act
            var strategy = sut.Strategy;

            //Assert
            strategy.Should().Be(GetBlogNewsStrategy.GET_BY_NAME);
        }

        [Theory]
        [InlineData(true, "fake name", "Invalid strategy body")]
        [InlineData(false, "", "Invalid strategy body")]
        [InlineData(false, " ", "Invalid strategy body")]
        [InlineData(false, null, "Invalid strategy body")]
        public void RunAsync_AllInvalidCases_ShouldThrow(bool bodyIsNull, string name, string exceptionMessage)
        {
            //Arrange
            var body = bodyIsNull ? null : new GetBlogNewsStrategyBody(name);

            var sut = new GetByNameStrategy(_logger, _unitOfWork, _mapper);

            //Act
            var act = async () => await sut.RunAsync(body, CancellationToken.None);

            //Assert
            act.Should().ThrowExactlyAsync<BusinessException>().WithMessage(exceptionMessage);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(0)]
        public void RunAsync_AllSuccessCases_ShouldReturnBlogNewViewModelCollection(int resultCount)
        {
            //Arrange
            var body = new GetBlogNewsStrategyBody("fake name");

            _mapper.Map<IEnumerable<BlogNewViewModel>>(Arg.Any<IEnumerable<BlogNew>>())
                   .Returns(_fixture.BlogNewViewModelFaker.Generate(resultCount));

            _blogNewsRepository.GetByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                               .Returns(_fixture.BlogNewFaker.Generate(resultCount));

            var sut = new GetByNameStrategy(_logger, _unitOfWork, _mapper);

            //Act
            var response = (IEnumerable<BlogNewViewModel>)sut.RunAsync(body, CancellationToken.None).Result;

            //Assert
            response.Count().Should().Be(resultCount);
        }
    }
}
