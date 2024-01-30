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
using TechBlog.NewsManager.Tests.UnitTests.Fixtures;

namespace TechBlog.NewsManager.Tests.UnitTests.Application.UseCases.BlogUsers
{
    public class CreateTests
    {
        private readonly IIdentityManager _identityManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateBlogUserRequest> _validator;

        public CreateTests()
        {
            _identityManager = Substitute.For<IIdentityManager>();
            _logger = Substitute.For<ILoggerManager>();
            _mapper = Substitute.For<IMapper>();
            _validator = Substitute.For<IValidator<CreateBlogUserRequest>>();
        }

        [Theory]
        [InlineData("", "", "", BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData(null, null, null, BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData("", null, null, BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData("", "", null, BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData("", null, "", BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData(null, "", "", BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData(null, "", null, BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData(null, null, "", BlogUserType.READER, false, "Invalid email", "Invalid name", "Invalid password")]
        [InlineData("invalidemail", "valid!@3", "Valid Name", BlogUserType.READER, false, "Invalid email")]
        [InlineData("valid@email.com", "", "Valid Name", BlogUserType.READER, false, "Invalid password")]
        [InlineData("valid@email.com", "valid!@3", "", BlogUserType.READER, false, "Invalid name")]
        [InlineData("valid@email.com", "", "", BlogUserType.READER, false, "Invalid name", "Invalid password")]
        [InlineData("valid@email.com", "valid!@3", "Valid Name", BlogUserType.READER, true)]
        public void Validator_AllCases_SholdMatchResponse(string email, string password, string name, BlogUserType blogUserType, bool expectedValid, params string[] responseMessages)
        {
            //Arrange
            var request = new CreateBlogUserRequest(email, password, name, blogUserType);

            var sut = new CreateBlogUserValidator();

            //Act
            var response = sut.Validate(request);

            //Assert
            response.IsValid.Should().Be(expectedValid);

            foreach (var error in response.Errors.Select(e => e.ErrorMessage))
                responseMessages.Should().Contain(error);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(true, true, false)]
        [InlineData(false, false, false)]
        public void Action_AllCases_ShouldCoverAllCases(bool userExists, bool successCreatingUser, bool expectedSuccess)
        {
            //Arrange
            var request = new CreateBlogUserRequest("valid@email.com", "valid!@3", "Valid Name", BlogUserType.READER);
            _mapper.Map<BlogUser>(Arg.Any<CreateBlogUserRequest>()).Returns(new BlogUser
            {
                Name = request.Name,
                Email = request.Email,
                UserName = request.Email,
                BlogUserType = request.BlogUserType
            });
            _identityManager.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(userExists));
            _identityManager.CreateUserAsync(Arg.Any<BlogUser>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(successCreatingUser));

            //Act
            var response = CreateBlogUserHandler.Action(_identityManager, _logger, _mapper, _validator, request, CancellationToken.None).Result;
            var responseContext = HttpContextFixtures.GetResposeHttpContext(response);
            var responseBody = HttpContextFixtures.GetObjectFromBodyAsync<BaseResponse>(responseContext).Result;

            //Assert
            responseContext.Response.StatusCode.Should().Be(expectedSuccess ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest);

            responseBody.Should().NotBeNull();
            responseBody.Success.Should().Be(expectedSuccess);
        }
    }
}
