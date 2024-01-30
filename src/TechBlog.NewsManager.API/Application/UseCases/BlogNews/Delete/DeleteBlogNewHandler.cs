using System.Security.Claims;
using TechBlog.NewsManager.API.Domain.Database;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Extensions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Application.UseCases.BlogNews.Delete
{
    public static class DeleteBlogNewHandler
    {
        public static string Route => "/api/v1/blognew/{id:guid}";

        public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };

        public static Delegate Handle => Action;

        /// <summary>
        /// The action that deletes a blog new.
        /// </summary>
        /// <param name="logger">The logger manager.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="user">The user making the request.</param>
        /// <param name="id">The ID of the blog new to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200" cref="BaseResponse">Success</response>
        /// <response code="403" cref="BaseResponse">When the user is not allowed to delete the blog new</response>
        /// <response code="404" cref="BaseResponse">When the blog new was not found</response>
        /// <returns>The result of the delete blog new action.</returns>
        public static async Task<IResult> Action(ILoggerManager logger,
                                                    IUnitOfWork unitOfWork,
                                                    ClaimsPrincipal user,
                                                    Guid id,
                                                    CancellationToken cancellationToken)
        {
            var response = new BaseResponse();

            var blogNew = await unitOfWork.BlogNew.GetByIdAsync(id, cancellationToken);

            if (!blogNew.IsEnabled(logger, response, out var notFoundResult))
                return notFoundResult;

            if (!blogNew.IsTheOwner(user, logger, out var forbidResult))
                return forbidResult;

            await unitOfWork.BlogNew.DeleteAsync(id, cancellationToken);

            logger.Log("Blog new deleted", LoggerManagerSeverity.DEBUG, ("Id", id));

            return Results.Ok(response.AsSuccess());
        }
    }
}