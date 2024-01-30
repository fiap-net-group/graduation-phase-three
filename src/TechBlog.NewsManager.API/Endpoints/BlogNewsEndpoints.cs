using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Create;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Delete;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.GetByStrategy;
using TechBlog.NewsManager.API.Application.UseCases.BlogNews.Update;
using TechBlog.NewsManager.API.Application.ViewModels;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Endpoints
{
    public static class BlogNewsEndpoints
    {
        public static IEndpointRouteBuilder MapBlogNewsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapMethods(CreateBlogNewHandler.Route, CreateBlogNewHandler.Methods, CreateBlogNewHandler.Handle)
              .WithTags("Blog New")
              .WithDescription("Create a new Blog New")
              .WithDisplayName("Create Blog New")
              .ProducesValidationProblem()
              .RequireAuthorization(AuthorizationPolicies.IsJournalist)
              .Produces<BaseResponse>(StatusCodes.Status200OK)
              .Produces<BaseResponse>(StatusCodes.Status400BadRequest)
              .Produces<BaseResponse>(StatusCodes.Status401Unauthorized);

            endpoints.MapMethods(GetByStrategyHandler.Route, GetByStrategyHandler.Methods, GetByStrategyHandler.Handle)
              .WithTags("Blog New")
              .WithDescription("Gets the blog news by a defined strategy")
              .WithDisplayName("Gets blog news by strategy")
              .ProducesValidationProblem()
              .Produces<BaseResponseWithValue<BlogNewViewModel[]>>(StatusCodes.Status200OK)
              .Produces<BaseResponse>(StatusCodes.Status400BadRequest)
              .Produces<BaseResponse>(StatusCodes.Status404NotFound)
              .Produces<BaseResponse>(StatusCodes.Status401Unauthorized);

            endpoints.MapMethods(DeleteBlogNewHandler.Route, DeleteBlogNewHandler.Methods, DeleteBlogNewHandler.Handle)
              .WithTags("Blog New")
              .WithDescription("Delete a blog new")
              .WithDisplayName("Delete blog new")
              .ProducesValidationProblem()
              .RequireAuthorization(AuthorizationPolicies.IsJournalist)
              .Produces<BaseResponse>(StatusCodes.Status200OK)
              .Produces<BaseResponse>(StatusCodes.Status400BadRequest)
              .Produces<BaseResponse>(StatusCodes.Status401Unauthorized);

            endpoints.MapMethods(UpdateBlogNewHandler.Route, UpdateBlogNewHandler.Methods, UpdateBlogNewHandler.Handle)
              .WithTags("Blog New")
              .WithDescription("Update a blog new")
              .WithDisplayName("Update blog new")
              .ProducesValidationProblem()
              .RequireAuthorization(AuthorizationPolicies.IsJournalist)
              .Produces<BaseResponse>(StatusCodes.Status200OK)
              .Produces<BaseResponse>(StatusCodes.Status400BadRequest)
              .Produces<BaseResponse>(StatusCodes.Status401Unauthorized);

            return endpoints;
        }
    }
}
