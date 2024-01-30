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
    public class GetByIdStrategyTests
    {
        private readonly UnitTestsFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IBlogNewsRepository _blogNewsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetByIdStrategyTests(UnitTestsFixture fixture)
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
            var sut = new GetByIdStrategy(_logger, _unitOfWork, _mapper);

            //Act
            var strategy = sut.Strategy;

            //Assert
            strategy.Should().Be(GetBlogNewsStrategy.GET_BY_ID);
        }

        [Theory]
        [InlineData(true, "909080d1-c3c2-4c5c-a849-6deae16f2619", true, "Invalid strategy body")]
        [InlineData(false, "00000000-0000-0000-0000-000000000000", true, "Invalid strategy body")]
        [InlineData(false, "909080d1-c3c2-4c5c-a849-6deae16f2619", false, "Blog new doesn't exists")]
        public void RunAsync_AllInvalidCases_ShouldThrow(bool bodyIsNull, string id, bool existsOnDatabase, string exceptionMessage)
        {
            //Arrange
            var body = bodyIsNull ? null : new GetBlogNewsStrategyBody(Guid.Parse(id));

            _blogNewsRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                               .Returns(_fixture.BlogNewFaker.RuleFor(b => b.Enabled, existsOnDatabase).Generate(1)[0]);

            var sut = new GetByIdStrategy(_logger, _unitOfWork, _mapper);

            //Act
            var act = async () => await sut.RunAsync(body, CancellationToken.None);

            //Assert
            act.Should().ThrowExactlyAsync<BusinessException>().WithMessage(exceptionMessage);
        }

        [Fact]
        public void RunAsync_BlogNewFound_ShouldReturnViewModel()
        {
            //Arrange
            var body = new GetBlogNewsStrategyBody(Guid.NewGuid());

            _mapper.Map<BlogNewViewModel>(Arg.Any<BlogNew>())
                   .Returns(_fixture.BlogNewViewModelFaker.Generate(1)[0]);

            _blogNewsRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                               .Returns(_fixture.BlogNewFaker.RuleFor(b => b.Enabled, true).Generate(1)[0]);

            var sut = new GetByIdStrategy(_logger, _unitOfWork, _mapper);

            //Act
            var response = (BlogNewViewModel)sut.RunAsync(body, CancellationToken.None).Result;

            //Assert
            response.Should().NotBeNull();
        }
    }
}
