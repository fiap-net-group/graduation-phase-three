using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Create;
using TechBlog.NewsManager.API.Domain.Database;
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
    public class CreateBlogNewsStepDefinitions
    {
        private readonly UnitTestsFixture _fixture;

        private CreateBlogNewRequest request;
        private BlogUserType blogUserType;

        private IResult _response;
        private HttpContext _httpContext;
        private BaseResponse _baseResponse;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private ClaimsPrincipal _claimsPrincipal;
        private readonly IValidator<CreateBlogNewRequest> _validator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBlogNewsStepDefinitions(UnitTestsFixture fixture)
        {
            _fixture = fixture;

            _logger = Substitute.For<ILoggerManager>();
            _mapper = Substitute.For<IMapper>();
            _claimsPrincipal = Substitute.For<ClaimsPrincipal>();
            _validator = Substitute.For<IValidator<CreateBlogNewRequest>>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
        }

        [Given(@"I wanted to create a blog news as ""([^""]*)""")]
        public void GivenIWantedToCreateABlogNewsAs(string jOURNALIST)
        {
            blogUserType = Enum.Parse<BlogUserType>(jOURNALIST);

            request = new CreateBlogNewRequest();
        }

        [Given(@"with title ""([^""]*)"" and description ""([^""]*)"" and body ""([^""]*)"" and enabled ""([^""]*)""")]
        public void GivenWithTitleAndDescriptionAndBodyAndEnabled(string title, string description, string body, string enabled)
        {
            request.Title = title;
            request.Description = description;
            request.Body = body;
            request.Enabled = bool.Parse(enabled);
        }

        [Given(@"tags:")]
        public void GivenTags(Table table)
        {
            request.Tags = table.Rows.Select(x => x.Values.First()).ToArray();

        }

        [When(@"I send a request to create a blog news")]
        public void WhenISendARequestToCreateABlogNews()
        {
            _mapper.Map<BlogNew>(Arg.Any<CreateBlogNewRequest>()).Returns(new BlogNew
            {
                Author = new BlogUser { Id = Guid.NewGuid().ToString(), Name = "Author Test" },
                Body = request.Body,
                CreatedAt = DateTime.Now,
                Description = request.Description,
                Enabled = request.Enabled,
                Id = Guid.NewGuid(),
                InternalTags = string.Join(";", request.Tags),
                LastUpdateAt = DateTime.Now,
                Title = request.Title,
                Tags = request.Tags
            });

            _claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>{
                new ClaimsIdentity(new List<Claim>{
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, "Author Test"),
                    new Claim("BlogUserType", blogUserType.ToString())
                })
            });

            var entity = _fixture.BlogNew.GenerateBlogNew(request);

            _unitOfWork.BlogNew.AddAsync(entity, CancellationToken.None).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(true);

            _response = CreateBlogNewHandler.Action(_logger, _mapper, _unitOfWork, request, _claimsPrincipal, _validator, CancellationToken.None).Result;


            _httpContext = _fixture.HttpContext.GetResposeHttpContext(_response);
            _baseResponse = _fixture.HttpContext.GetObjectFromBodyAsync<BaseResponse>(_httpContext).Result;
        }

        [Then(@"I should get a response to create create a blog news with status code (.*)")]
        public void ThenIShouldGetAResponseToCreateCreateABlogNewsWithStatusCode(int p0)
        {
            _httpContext.Response.StatusCode.Should().Be(p0);

            _baseResponse.Should().NotBeNull();
        }

        [Then(@"result success to create a blog news is ""([^""]*)""")]
        public void ThenResultSuccessToCreateABlogNewsIs(string @true)
        {
            _baseResponse.Success.Should().Be(bool.Parse(@true));
        }

        [Then(@"message to create a blog news is ""([^""]*)""")]
        public void ThenMessageToCreateABlogNewsIs(string p0)
        {
            Assert.Equal(p0, _baseResponse.ResponseDetails.Message);
        }

    }
}
