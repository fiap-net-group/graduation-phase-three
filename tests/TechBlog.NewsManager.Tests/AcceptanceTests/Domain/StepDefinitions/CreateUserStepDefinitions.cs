using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.API.Domain.ValueObjects;
using TechBlog.NewsManager.Tests.AcceptanceTests.Fixtures;
using TechTalk.SpecFlow;
using ValidationResultFluent = FluentValidation.Results.ValidationResult;

namespace TechBlog.NewsManager.AcceptanceTests.Domain.StepDefinitions
{
    [Binding]
    public class CreateUserStepDefinitions
    {
        private readonly UnitTestsFixture _fixture;

        private CreateBlogUserRequest request;
        private IResult _response;
        private HttpContext _httpContext;
        private BaseResponse _baseResponse;
        private ValidationResultFluent _validationResult;

        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IValidator<CreateBlogUserRequest> _validator;
        private readonly IIdentityManager _identityManager;

        public CreateUserStepDefinitions(UnitTestsFixture fixture)
        {
            _fixture = fixture;

            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILoggerManager>();
            _validator = Substitute.For<IValidator<CreateBlogUserRequest>>();
            _identityManager = Substitute.For<IIdentityManager>();
        }


        [Given(@"I wanted to create a new user")]
        public void GivenIWantedToCreateANewUser()
        {
            request = new CreateBlogUserRequest();
        }

        [Given(@"with email ""([^""]*)"" and password ""([^""]*)"" and name ""([^""]*)"" and blogUserType ""([^""]*)""")]
        public void GivenWithEmailAndPasswordAndNameAndBlogUserType(string p0, string p1, string p2, string p3)
        {
            var blogUserType = Enum.Parse<BlogUserType>(p3);

            request.Email = p0;
            request.Password = p1;
            request.Name = p2;
            request.BlogUserType = blogUserType;
        }

        [When(@"I send a request to create a new user that exists is ""([^""]*)"" and success to create is ""([^""]*)""")]
        public void WhenISendARequestToCreateANewUserThatExistsIsAndSuccessToCreateIs(string t0, string t1)
        {
            _mapper.Map<BlogUser>(Arg.Any<CreateBlogUserRequest>()).Returns(new BlogUser
            {
                Name = request.Name,
                Email = request.Email,
                UserName = request.Email,
                BlogUserType = request.BlogUserType
            });

            _identityManager.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(bool.Parse(t0)));
            _identityManager.CreateUserAsync(Arg.Any<BlogUser>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(bool.Parse(t1)));

            _response = CreateBlogUserHandler.Action(_identityManager, _logger, _mapper, _validator, request, CancellationToken.None).Result;
            _httpContext = _fixture.HttpContext.GetResposeHttpContext(_response);
            _baseResponse = _fixture.HttpContext.GetObjectFromBodyAsync<BaseResponse>(_httpContext).Result;
        }


        [Then(@"I should get a response to create a new user with status code (.*)")]
        public void ThenIShouldGetAResponseToCreateANewUserWithStatusCode(int p0)
        {
            Assert.Equal(p0, _httpContext.Response.StatusCode);
        }

        [When(@"I validate request to create a new user")]
        public void WhenIValidateRequestToCreateANewUser()
        {
            var sut = new CreateBlogUserValidator();

            _validationResult = sut.Validate(request);
        }

        [Then(@"result success is ""([^""]*)""")]
        public void ThenResultSuccessIs(string @true)
        {
            _response.Should().NotBeNull();
            _baseResponse.Success.Should().Be(bool.Parse(@true));
        }

        [Then(@"I should get a validate error to create a new user with this message ""([^""]*)""")]
        public void ThenIShouldGetAValidateErrorToCreateANewUserWithThisMessage(string p0)
        {
            Assert.Equal(p0, _validationResult.Errors[0].ErrorMessage);
        }
    }
}
