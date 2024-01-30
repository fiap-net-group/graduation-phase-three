using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using TechBlog.NewsManager.API.Application.UseCases.Authentication.Login;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.Tests.AcceptanceTests.Fixtures;
using TechTalk.SpecFlow;
using ValidationResultFluent = FluentValidation.Results.ValidationResult;

namespace TechBlog.NewsManager.AcceptanceTests.Domain.StepDefinitions
{
    [Binding]
    public class LoginStepDefinitions
    {
        private readonly UnitTestsFixture _fixture;

        private LoginRequest _loginRequest;
        private IResult _response;
        private HttpContext _httpContext;
        private BaseResponseWithValue<AccessTokenModel> _baseResponse;
        private ValidationResultFluent _validationResult;

        private readonly ILoggerManager _logger;
        private readonly IValidator<LoginRequest> _validator;
        private readonly IIdentityManager _identityManager;

        public LoginStepDefinitions(UnitTestsFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<LoginRequest>>();
            
            _identityManager = Substitute.For<IIdentityManager>();
        }

        [Given(@"I wanted to login with username ""([^""]*)"" and password ""([^""]*)""")]
        public void GivenIWantedToLoginWithUsernameAndPassword(string user, string password)
        {
            _loginRequest = new LoginRequest(user,password);
        }

        [When(@"I send the request that user exists ""([^""]*)"" and password is correct ""([^""]*)""")]
        public void WhenISendTheRequestThatUserExistsAndPasswordIsCorrect(string p1, string p2)
        {
            bool exists = Boolean.Parse(p1);
            bool passwordMatches = Boolean.Parse(p2);

            _identityManager.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(
                    exists ? new BlogUser
                    {
                        InternalId = Guid.NewGuid(),
                        Email = "validusername@email.com"
                    }: new BlogUser(valid: false)
                ));

            _identityManager.AuthenticateAsync(Arg.Any<BlogUser>(), Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<(string, string)[]>())
                            .Returns(Task.FromResult(_fixture.Authorization.GenerateViewModel(passwordMatches)));

            _response = LoginHandler.Action(_logger, _validator, _identityManager, _loginRequest, CancellationToken.None).Result;
            _httpContext = _fixture.HttpContext.GetResposeHttpContext(_response);
            _baseResponse = _fixture.HttpContext.GetObjectFromBodyAsync<BaseResponseWithValue<AccessTokenModel>>(_httpContext).Result;
        }

        [Then(@"I wish return with success ""([^""]*)"" and status code ""([^""]*)""")]
        public void ThenIWishReturnWithSuccessAndStatusCode(string @true, string p1)
        {
            _httpContext.Response.StatusCode.Should().Be(int.Parse(p1));

            _baseResponse.Should().NotBeNull();
            _baseResponse.Success.Should().Be(Boolean.Parse(@true));
            if (Boolean.Parse(@true))
                _baseResponse.Value.Valid.Should().BeTrue();
        }

        [When(@"I validate that user")]
        public void WhenIValidateThatUser()
        {
            var loginValidator = new LoginValidator();
            _validationResult = loginValidator.Validate(_loginRequest);
        }

        [Then(@"I wish the validator response with success ""([^""]*)"" and message error is ""([^""]*)""")]
        public void ThenIWishTheValidatorResponseWithSuccessAndMessageErrorIs(string @false, string p1)
        {
            _validationResult.IsValid.Should().Be(Boolean.Parse(@false));

            foreach (var error in _validationResult.Errors.Select(e => e.ErrorMessage))
                p1.Should().Contain(error);
        }


    }
}
