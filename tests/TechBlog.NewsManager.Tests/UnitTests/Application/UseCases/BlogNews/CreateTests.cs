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
using TechBlog.NewsManager.Tests.UnitTests.Fixtures;

namespace TechBlog.NewsManager.Tests.UnitTests.Application.UseCases.BlogNews
{
    public class CreateTests
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBlogNewRequest> _validator;
        private readonly IUnitOfWork _unitOfWork;
        private ClaimsPrincipal _claimsPrincipal;
        public CreateTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _mapper = Substitute.For<IMapper>();
            _validator = Substitute.For<IValidator<CreateBlogNewRequest>>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _claimsPrincipal = Substitute.For<ClaimsPrincipal>();
        }

        [Theory]
        [InlineData("", "", "", new string[] { }, false, false, new string[] { "Invalid title", "Invalid description", "Invalid body", "Invalid tags" })]
        [InlineData(null, null, null, new string[] { }, false, false, new string[] { "Invalid title", "Invalid description", "Invalid body", "Invalid tags" })]
        [InlineData("", null, null, new string[] { }, false, false, new string[] { "Invalid title", "Invalid description", "Invalid body", "Invalid tags" })]
        [InlineData("", "", null, new string[] { }, false, false, new string[] { "Invalid title", "Invalid description", "Invalid body", "Invalid tags" })]
        [InlineData("", "", "", new string[] { }, true, false, new string[] { "Invalid title", "Invalid description", "Invalid body", "Invalid tags" })]
        [InlineData("Title Teste", "", "", new string[] { }, false, false, new string[] { "Invalid description", "Invalid body", "Invalid tags" })]
        [InlineData("Title Test", "Description Test", "", new string[] { }, false, false, new string[] { "Invalid body", "Invalid tags" })]
        [InlineData("Title Test", "Description Test", "Body Test", new string[] { }, false, false, new string[] { "Invalid tags" })]
        [InlineData("Title Test", "Description Test", "Body Test", new string[] { "test tag" }, false, true, new string[] { })]

        public void Validator_AllCases_SholdMatchResponse(string title, string description, string body, string[] tags, bool enabled, bool expectedValid, string[] responseMessages)
        {
            //Arrange
            var request = new CreateBlogNewRequest { Title = title, Description = description, Body = body, Tags = tags, Enabled = enabled };

            var sut = new CreateBlogNewValidator();

            //Act
            var response = sut.Validate(request);

            //Assert
            response.IsValid.Should().Be(expectedValid);
            response.Errors.Select(x => x.ErrorMessage).Should().BeEquivalentTo(responseMessages);
        }

        [Theory]
        [InlineData(BlogUserType.READER, StatusCodes.Status400BadRequest, false)]
        [InlineData(BlogUserType.JOURNALIST, StatusCodes.Status201Created, true)]
        public void Action_AllCases_ShouldCoverAllCases(BlogUserType blogUserType, int statusCode, bool expectedSuccess)
        {
            //Arrange
            var request = new CreateBlogNewRequest { Title = "Title Test", Description = "Description Test", Body = "Body Test", Tags = new string[] { "test tag" }, Enabled = true };

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

            var entity = BlogNewFixtures.GenerateBlogNew(request);

            _unitOfWork.BlogNew.AddAsync(entity, CancellationToken.None).Returns(Task.CompletedTask);
            _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(expectedSuccess);

            //Act
            var response = CreateBlogNewHandler.Action(_logger, _mapper, _unitOfWork, request, _claimsPrincipal, _validator, CancellationToken.None).Result;


            var responseContext = HttpContextFixtures.GetResposeHttpContext(response);
            var responseBody = HttpContextFixtures.GetObjectFromBodyAsync<BaseResponse>(responseContext).Result;


            //Assert
            responseContext.Response.StatusCode.Should().Be(statusCode);

            responseBody.Should().NotBeNull();
            responseBody.Success.Should().Be(expectedSuccess);
        }

    }
}