using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using TechBlog.NewsManager.API.Application.UseCases.Authentication.Login;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.Tests.UnitTests.Fixtures;

namespace TechBlog.NewsManager.Tests.UnitTests.Application.UseCases.Authentication
{
    [Collection(nameof(UnitTestsFixtureCollection))]
    public class LoginTests
    {
        private readonly UnitTestsFixture _fixture;

        private readonly ILoggerManager _logger;
        private readonly IValidator<LoginRequest> _validator;
        private readonly IIdentityManager _identityManager;

        public LoginTests(UnitTestsFixture fixture)
        {
            _fixture = fixture;
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<LoginRequest>>();
            _identityManager = Substitute.For<IIdentityManager>();
        }

        [Theory]
        [InlineData("", "", false, "Invalid email", "Invalid password")]
        [InlineData(null, null, false, "Invalid email", "Invalid password")]
        [InlineData("", null, false, "Invalid email", "Invalid password")]
        [InlineData(null, "", false, "Invalid email", "Invalid password")]
        [InlineData("username@email.com", "", false, "Invalid password")]
        [InlineData("", "password123", false, "Invalid email")]
        [InlineData("username@email.com", "password123", true)]
        public void Validator_AllCases_SholdMatchResponse(string username, string password, bool expectedValid, params string[] responseMessages)
        {
            //Arrange
            var request = new LoginRequest(username, password);

            var sut = new LoginValidator();

            //Act
            var response = sut.Validate(request);

            //Assert
            response.IsValid.Should().Be(expectedValid);

            foreach (var error in response.Errors.Select(e => e.ErrorMessage))
                responseMessages.Should().Contain(error);
        }

        [Theory]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        public void Action_AllCases_ShouldReturnExpectedResponse(bool existsOnDatabase, bool matchesCredentials, bool success)
        {
            //Arrange
            _identityManager.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(
                existsOnDatabase ? new BlogUser
                {
                    InternalId = Guid.NewGuid(),
                    Email = "validusername@email.com"
                }
                : new BlogUser(valid: false)));

            _identityManager.AuthenticateAsync(Arg.Any<BlogUser>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<(string, string)[]>())
                            .Returns(Task.FromResult(_fixture.Authorization.GenerateViewModel(matchesCredentials)));

            var request = new LoginRequest("validusername@email.com", "validPassword!23");

            //Act
            var response = LoginHandler.Action(_logger, _validator, _identityManager, request, CancellationToken.None).Result;
            var responseContext = HttpContextFixtures.GetResposeHttpContext(response);
            var responseBody = HttpContextFixtures.GetObjectFromBodyAsync<BaseResponseWithValue<AccessTokenModel>>(responseContext).Result;

            //Assert
            responseContext.Response.StatusCode.Should().Be(success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest);

            responseBody.Should().NotBeNull();
            responseBody.Success.Should().Be(success);
            if (success)
                responseBody.Value.Valid.Should().BeTrue();
        }
    }
}
