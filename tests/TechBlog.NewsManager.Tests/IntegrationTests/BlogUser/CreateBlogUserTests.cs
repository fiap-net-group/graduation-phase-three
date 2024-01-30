using FluentAssertions;
using System.Net.Http.Json;
using TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.Tests.IntegrationTests.Fixtures;

namespace TechBlog.NewsManager.Tests.IntegrationTests.BlogUser
{
    [Collection(nameof(IntegrationTestsFixtureCollection))]
    public class CreateBlogUserTests
    {
        private readonly IntegrationTestsFixture _fixture;

        public CreateBlogUserTests(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task RequestWithoutAccessKey_ShouldReturnUnauthorized()
        {
            //Arrange
            var body = new CreateBlogUserRequest
                (
                    email: IntegrationTestsHelper.JournalistEmail,
                    password: IntegrationTestsHelper.FakePassword,
                    name: IntegrationTestsHelper.JournalistName,
                    blogUserType: API.Domain.ValueObjects.BlogUserType.JOURNALIST
                );

            _fixture.WithoutApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(CreateBlogUserHandler.Route, body);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse>();
            responseBody.Success.Should().BeFalse();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.GenericError.GetDescription());
        }

        [Fact]
        public async Task ValidRequest_ShouldReturnCreated()
        {
            //Arrange
            var body = new CreateBlogUserRequest
                (
                    email: IntegrationTestsHelper.JournalistEmail,
                    password: IntegrationTestsHelper.FakePassword,
                    name: IntegrationTestsHelper.JournalistName,
                    blogUserType: API.Domain.ValueObjects.BlogUserType.JOURNALIST
                );

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(CreateBlogUserHandler.Route, body);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponseWithValue<AccessTokenModel>>();
            responseBody.Success.Should().BeTrue();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.Success.GetDescription());

            await _fixture.ClearDb();
        }

        [Fact]
        public async Task ValidRequest_ButUserAlreadyExists_ShouldReturnBadRequest()
        {
            //Arrange
            var body = new CreateBlogUserRequest
                (
                    email: IntegrationTestsHelper.JournalistEmail,
                    password: IntegrationTestsHelper.FakePassword,
                    name: IntegrationTestsHelper.JournalistName,
                    blogUserType: API.Domain.ValueObjects.BlogUserType.JOURNALIST
                );

            await _fixture.CreateJournalist();

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(CreateBlogUserHandler.Route, body);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse>();
            responseBody.Success.Should().BeFalse();
            responseBody.ResponseDetails.Message.Should().Be(ResponseMessage.UserAlreadyExists.GetDescription());

            await _fixture.ClearDb();
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("", "", "", ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData(null, null, null, ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData("", null, null, ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData("", "", null, ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData("", null, "", ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData(null, "", "", ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData(null, "", null, ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData(null, null, "", ResponseMessage.InvalidEmail, ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        [InlineData("invalidemail", "valid!@3", "Valid Name", ResponseMessage.InvalidEmail)]
        [InlineData("valid@email.com", "", "Valid Name", ResponseMessage.InvalidPassword)]
        [InlineData("valid@email.com", "valid!@3", "", ResponseMessage.InvalidName)]
        [InlineData("valid@email.com", "", "", ResponseMessage.InvalidName, ResponseMessage.InvalidPassword)]
        public async Task InvalidRequest_ShouldReturnBadRequest(string email, string password, string name, params ResponseMessage[] detailsMessage)
        {
            //Arrange
            var body = new CreateBlogUserRequest
                (
                    email: email,
                    password: password,
                    name: name,
                    blogUserType: API.Domain.ValueObjects.BlogUserType.JOURNALIST
                );

            _fixture.WithApiKeyHeader();

            //Act
            var response = await _fixture.Client.PostAsJsonAsync(CreateBlogUserHandler.Route, body);

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
