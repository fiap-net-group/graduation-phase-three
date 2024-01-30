using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TechBlog.NewsManager.API.Endpoints;
using TechBlog.NewsManager.API.Middlewares;
using TechBlog.NewsManager.API.Middlewares.Logger;

namespace TechBlog.NewsManager.API.DependencyInjection.Configurations
{
    [ExcludeFromCodeCoverage]
    public static class ApiConfiguration
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this WebApplication app, bool isDevelopment)
        {
            app.UseRouting();

            app.UseMiddlewareIfNotDevelopment<LoggerMiddleware>(isDevelopment);
            app.UseMiddlewareIfNotDevelopment<LogRequestMiddleware>(isDevelopment);
            app.UseMiddlewareIfNotDevelopment<LogResponseMiddleware>(isDevelopment);

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseMiddlewareIfNotDevelopment<ApiKeyMiddleware>(isDevelopment);

            app.UseApplicationEndpoints();


            return app;
        }

        private static IApplicationBuilder UseMiddlewareIfNotDevelopment<T>(this WebApplication app, bool isDevelopment)
        {
            if (!isDevelopment)
                app.UseMiddleware<T>();

            return app;
        }
    }
}
