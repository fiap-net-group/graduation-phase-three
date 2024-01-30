using TechBlog.NewsManager.API.Application.UseCases.BlogUsers.Create;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Endpoints
{
    public static class BlogUserEndpoints
    {
        public static IEndpointRouteBuilder MapBlogUsersEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapMethods(CreateBlogUserHandler.Route, CreateBlogUserHandler.Methods, CreateBlogUserHandler.Handle)
              .WithTags("Users")
              .WithDescription("Create a new Blog User")
              .WithDisplayName("Create Blog User")
              .ProducesValidationProblem()
              .Produces<BaseResponseWithValue<AccessTokenModel>>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status400BadRequest);

            return endpoints;
        }
    }
}
