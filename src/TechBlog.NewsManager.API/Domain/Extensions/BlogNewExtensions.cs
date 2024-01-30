using Azure;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TechBlog.NewsManager.API.Domain.Extensions
{
    public static class BlogNewExtensions
    {
        public static bool IsEnabled(this BlogNew blogNew, ILoggerManager logger, BaseResponse response, out IResult result)
        {
            result = default;

            if (blogNew.Enabled)
                return blogNew.Enabled;

            logger.Log("Blog new not found", LoggerManagerSeverity.DEBUG, ("Id", blogNew.Id));

            result = Results.NotFound(response.AsError(ResponseMessage.BlogNewNotFound));

            return blogNew.Enabled;
        }

        public static bool IsTheOwner(this BlogNew blogNew, ClaimsPrincipal user, ILoggerManager logger, out IResult result)
        {
            result = default;

            if (blogNew.AuthorId == user.FindFirstValue(ClaimTypes.NameIdentifier))
                return true;

            logger.Log("User not allowed to action", LoggerManagerSeverity.INFORMATION, ("Id", blogNew.Id), ("UserId", user.FindFirstValue(ClaimTypes.NameIdentifier)), ("BlogNewAuthorId", blogNew.AuthorId));

            result = Results.Forbid();

            return false;
        }
    }
}
