using FluentAssertions;
using System.Net.Http.Json;
using TechBlog.NewsManager.API.Application.UseCases.Authentication.Login;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.Tests.IntegrationTests.Fixtures;

namespace TechBlog.NewsManager.Tests.IntegrationTests.Authentication
{
    [Collection(nameof(IntegrationTestsFixtureCollection))]
    public class LoginTests
    {
        private readonly IntegrationTestsFixture _fixture;

        public LoginTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RequestWithoutAccessKey_ShouldReturnUnauthorized()
        {
            //Arrange
            var body = new LoginRequest(IntegrationTestsHelper.JournalistEmail, IntegrationTestsHelper.FakePassword);

            _fixture.WithoutApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(LoginHandler.Route, body);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse>();
            responseBody.Success.Should().BeFalse();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.GenericError.GetDescription());
        }

        [Fact]
        public async Task ValidRequest_ShouldReturnAuthenticate()
        {
            //Arrange
            var body = new LoginRequest(IntegrationTestsHelper.JournalistEmail, IntegrationTestsHelper.FakePassword);

            await _fixture.CreateJournalist();

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(LoginHandler.Route, body);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponseWithValue<AccessTokenModel>>();
            responseBody.Success.Should().BeTrue();
            responseBody.Value.Valid.Should().BeTrue();

            await _fixture.ClearDb();
        }

        [Fact]
        public async Task ValidRequest_ButUserDontExists_ShouldReturnBadRequest()
        {
            //Arrange
            var body = new LoginRequest(IntegrationTestsHelper.JournalistEmail, IntegrationTestsHelper.FakePassword);

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(LoginHandler.Route, body);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse>();
            responseBody.Success.Should().BeFalse();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.InvalidCredentials.GetDescription());
        }

        [Theory]
        [InlineData("", "", ResponseMessage.InvalidEmail, ResponseMessage.InvalidPassword)]
        [InlineData(null, null, ResponseMessage.InvalidEmail, ResponseMessage.InvalidPassword)]
        [InlineData("", null, ResponseMessage.InvalidEmail, ResponseMessage.InvalidPassword)]
        [InlineData(null, "", ResponseMessage.InvalidEmail, ResponseMessage.InvalidPassword)]
        [InlineData("username@email.com", "", ResponseMessage.InvalidPassword)]
        [InlineData("", "password123", ResponseMessage.InvalidEmail)]
        public async Task InvalidRequest_ShouldReturnBadRequest(string username, string password, params ResponseMessage[] detailsMessage)
        {
            //Arrange
            var body = new LoginRequest(username, password);

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(LoginHandler.Route, body);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse>();
            responseBody.Success.Should().BeFalse();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.InvalidInformation.GetDescription());
            foreach (var error in detailsMessage)
                responseBody.ResponseDetails.Errors.Should().Contain(error.GetDescription());
        }
    }
}
