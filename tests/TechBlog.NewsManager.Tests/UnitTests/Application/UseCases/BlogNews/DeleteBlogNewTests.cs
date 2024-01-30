using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Security.Claims;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Delete;
using TechBlog.NewsManager.API.Domain.Database;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.Tests.UnitTests.Fixtures;

namespace TechBlog.NewsManager.Tests.UnitTests.Application.UseCases.BlogNews
{
    [Collection(nameof(UnitTestsFixtureCollection))]
    public class DeleteBlogNewHandlerTests
    {
        private readonly ILoggerManager _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CancellationToken _cancellationToken;

        public DeleteBlogNewHandlerTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _cancellationToken = CancellationToken.None;
        }

        [Fact]
        public async Task Action_WhenBlogNewExistsAndUserIsAuthor_ShouldDeleteBlogNew()
        {
            // Arrange
            var id = Guid.NewGuid();

            var blogNew = new BlogNew
            {
                Id = id,
                AuthorId = "author-id",
                Enabled = true
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "author-id")
            }));

            _unitOfWork.BlogNew.GetByIdAsync(id, _cancellationToken).Returns(blogNew);
            _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(true);

            // Act
            var result = await DeleteBlogNewHandler.Action(_logger, _unitOfWork, user, id, _cancellationToken);

            // Assert
            result.Should().BeOfType<Ok<BaseResponse>>();
            await _unitOfWork.BlogNew.Received(1).DeleteAsync(id, _cancellationToken);
        }

        [Fact]
        public async Task Action_WhenBlogNewExistsAndUserIsNotAuthor_ShouldReturnForbidResult()
        {
            // Arrange
            var id = Guid.NewGuid();

            var blogNew = new BlogNew
            {
                Id = id,
                AuthorId = "author-id",
                Enabled = true
            };

            _unitOfWork.BlogNew.GetByIdAsync(id, _cancellationToken).Returns(blogNew);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "not-author-id")
            }));

            // Act
            var result = await DeleteBlogNewHandler.Action(_logger, _unitOfWork, user, id, _cancellationToken);

            // Assert
            result.Should().BeOfType<ForbidHttpResult>();

            await _unitOfWork.BlogNew.DidNotReceiveWithAnyArgs().DeleteAsync(Guid.Empty, default);
            await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Fact]
        public async Task Action_WhenBlogNewDoesNotExist_ShouldReturnNotFoundResult()
        {
            // Arrange
            var id = Guid.NewGuid();

            _unitOfWork.BlogNew.GetByIdAsync(id, _cancellationToken).Returns(new BlogNew { Enabled = false });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "author-id")
            }));

            // Act
            var result = await DeleteBlogNewHandler.Action(_logger, _unitOfWork, user, id, _cancellationToken);
            // Assert
            result.Should().BeOfType<NotFound<BaseResponse>>();

            await _unitOfWork.BlogNew.DidNotReceiveWithAnyArgs().DeleteAsync(Guid.Empty, default);
            await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        }

        [Fact]
        public async Task Action_WhenUnitOfWorkFails_ShouldThrowInfrastructureException()
        {
            // Arrange
            var id = Guid.NewGuid();

            var blogNew = new BlogNew
            {
                Id = id,
                AuthorId = "author-id",
                Enabled = true
            };

            _unitOfWork.BlogNew.GetByIdAsync(id, _cancellationToken).Returns(blogNew);
            _unitOfWork.BlogNew.DeleteAsync(id, _cancellationToken).Throws(new InfrastructureException("Example message"));

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "author-id")
            }));

            // Act
            Func<Task> action = async () => await DeleteBlogNewHandler.Action(_logger, _unitOfWork, user, id, _cancellationToken);

            // Assert
            await action.Should().ThrowAsync<InfrastructureException>();
            await _unitOfWork.BlogNew.Received(1).DeleteAsync(id, _cancellationToken);
        }
    }
}