using AutoMapper;
using FluentValidation;
using System.Security.Claims;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Database;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using TechBlog.NewsManager.API.Domain.ValueObjects;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.Create
{
    public static class CreateBlogNewHandler
    {
        public static string Route => "/api/v1/blognew";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        /// <summary>
        /// Create a new BlogNew
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="request">The create new request body</param>
        /// <param name="user"></param>
        /// <param name="validator"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="201" cref="BaseResponse">BlogNew created</response>
        /// <response code="400" cref="BaseResponse">Invalid information</response>
        /// <response code="401" cref="BaseResponse">Unauthorized</response>
        /// <response code="403" cref="BaseResponse">User is not a Journalist</response>
        /// <returns></returns>
        public static async Task<IResult> Action(ILoggerManager logger,
                                                    IMapper mapper,
                                                    IUnitOfWork unitOfWork,
                                                    CreateBlogNewRequest request,
                                                    ClaimsPrincipal user,
                                                    IValidator<CreateBlogNewRequest> validator,
                                                    CancellationToken cancellationToken)
        {
            validator.ThrowIfInvalid(request);

            var response = new BaseResponseWithValue<BlogNewViewModel>();

            var blogNew = mapper.Map<BlogNew>(request);

            blogNew.AuthorId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            var userType = user.FindFirstValue("BlogUserType");

            if (string.IsNullOrWhiteSpace(userType) || userType != Enum.GetName(BlogUserType.JOURNALIST))
            {
                logger.Log("User is forbidden to create a new", LoggerManagerSeverity.INFORMATION, ("Title", request.Title), ("userId", blogNew.AuthorId));

                return Results.BadRequest(response.AsError(ResponseMessage.UserMustBeAJournalist));
            }

            await unitOfWork.BlogNew.AddAsync(blogNew, cancellationToken);

            if (!await unitOfWork.SaveChangesAsync(cancellationToken))
            {
                logger.Log("Error creating blog new", LoggerManagerSeverity.WARNING, ("Title", request.Title));

                throw new InfrastructureException("An unexpected error ocurred");
            }

            return Results.Created(Route, response.AsSuccess(mapper.Map<BlogNewViewModel>(blogNew)));
        }

    }
}