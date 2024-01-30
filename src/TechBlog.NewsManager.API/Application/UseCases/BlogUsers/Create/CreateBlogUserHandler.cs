using AutoMapper;
using FluentValidation;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create
{
    public static class CreateBlogUserHandler
    {
        public static string Route => "/api/v1/users";
        public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
        public static Delegate Handle => Action;

        /// <summary>
        /// Create a new BlogUser
        /// </summary>
        /// <param name="identityManager"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="validator"></param>
        /// <param name="request">The create blog user request body</param>
        /// <param name="cancellationToken"></param>
        /// <remarks>
        /// To use the endpoint, the user type must be one of the following:
        ///   - READER,
        ///   - JOURNALIST
        /// </remarks>
        /// <response code="201" cref="BaseResponse">BlogUser created</response>
        /// <response code="400" cref="BaseResponse">Invalid information or user already exists</response>
        /// <returns></returns>
        public static async Task<IResult> Action(IIdentityManager identityManager,
                                                 ILoggerManager logger,
                                                 IMapper mapper,
                                                 IValidator<CreateBlogUserRequest> validator,
                                                 CreateBlogUserRequest request,
                                                 CancellationToken cancellationToken)
        {
            logger.Log("Begin creating user", LoggerManagerSeverity.DEBUG, ("Email", request.Email));

            validator.ThrowIfInvalid(request);

            var response = new BaseResponseWithValue<AccessTokenModel>();

            var blogUser = mapper.Map<BlogUser>(request);

            if (await identityManager.ExistsAsync(blogUser.Email, cancellationToken))
            {
                logger.Log("User already exists", LoggerManagerSeverity.INFORMATION, ("Email", request.Email));

                return Results.BadRequest(response.AsError(ResponseMessage.UserAlreadyExists));
            }

            logger.Log("User don't exists, creting new", LoggerManagerSeverity.DEBUG, ("Email", request.Email));

            if (!await identityManager.CreateUserAsync(blogUser, request.Password, cancellationToken))
            {
                logger.Log("Error creating user", LoggerManagerSeverity.WARNING, ("Email", request.Email));

                return Results.BadRequest(response.AsError(ResponseMessage.InvalidInformation));
            }

            blogUser.WithInternalIdMapped();

            var accessToken = await identityManager.AuthenticateAsync(blogUser, request.Password, cancellationToken, ("name", blogUser.Name));

            logger.Log("Success creating user", LoggerManagerSeverity.DEBUG, ("Email", request.Email));

            return Results.Created(Route, response.AsSuccess(accessToken));
        }
    }
}
