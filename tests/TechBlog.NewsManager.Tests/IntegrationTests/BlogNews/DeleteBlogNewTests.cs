using FluentAssertions;
using System.Net.Http.Json;
using TechBlog.NewsManager.API.Application.UseCases.Authentication.Login;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Delete;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.Tests.IntegrationTests.Fixtures;

namespace TechBlog.NewsManager.Tests.IntegrationTests.BlogNews
{
    [Collection(nameof(IntegrationTestsFixtureCollection))]
    public class DeleteBlogNewTests
    {
        private readonly IntegrationTestsFixture _fixture;

        public DeleteBlogNewTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RequestWithoutAccessKey_ShouldReturnUnauthorized()
        {
            //Arrange
            var id = Guid.NewGuid();

            _fixture.WithoutApiKeyHeader();

            //Act
            var response = await _fixture.Client.DeleteAsync(DeleteBlogNewHandler.Route.Replace("{id:guid}", id.ToString()));

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse>();
            responseBody.Success.Should().BeFalse();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.GenericError.GetDescription());
        }

        [Fact]
        public async Task RequestWithoutAccessToken_ShouldReturnUnauthorized()
        {
            //Arrange
            var id = Guid.NewGuid();

            _fixture.WithoutAuthentication();
            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.DeleteAsync(DeleteBlogNewHandler.Route.Replace("{id:guid}", id.ToString()));

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse>();
            responseBody.Success.Should().BeFalse();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.UserIsNotAuthenticated.GetDescription());
        }

        [Fact]
        public async Task RequestWithReaderAccessToken_ShouldReturnForbidden()
        {
            //Arrange
            var id = Guid.NewGuid();

            await _fixture.AuthenticateAsync(API.Domain.ValueObjects.BlogUserType.READER);

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.DeleteAsync(DeleteBlogNewHandler.Route.Replace("{id:guid}", id.ToString()));

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

            await _fixture.ClearDb();
            _fixture.WithoutAuthentication();
        }


        [Fact]
        public async Task ValidRequest_ButJournalist_ShouldReturnBadRequest()
        {
            //Arrange
            _fixture.GenerateNewClient();
            var userId = await _fixture.AuthenticateAsync(API.Domain.ValueObjects.BlogUserType.JOURNALIST);
            var id = await _fixture.CreateBlogNew(userId);

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.DeleteAsync(DeleteBlogNewHandler.Route.Replace("{id:guid}", id));

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            _fixture.WithoutAuthentication();
            await _fixture.ClearDb();
        }

    }
}
