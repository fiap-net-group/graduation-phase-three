using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TechBlog.NewsManager.API.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using TechBlog.NewsManager.API.Infrastructure.Authentication.Configuration.Context;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using TechBlog.NewsManager.API.Endpoints;
using TechBlog.NewsManager.API.Middlewares;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TechBlog.NewsManager.Tests.IntegrationTests.Fixtures
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, DbConnection connection)
        {
            services.RemoveAll(typeof(DbContext));
            services.RemoveAll(typeof(IDatabaseContext));
            services.RemoveAll(typeof(SqlServerContext));
            services.RemoveAll(typeof(DbContextOptions<SqlServerContext>));

            services.AddScoped<IDatabaseContext, SqlServerContext>();
            services.AddDbContext<IDatabaseContext, SqlServerContext>(options =>
            {
                options.UseSqlite(connection);
            });

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services, DbConnection connection)
        {
            services.RemoveAll(typeof(DbContextOptions<IdentityContext>));
            services.RemoveAll(typeof(IdentityContext));
            services.RemoveAll(typeof(IIdentityContext));
            services.RemoveAll(typeof(IdentityDbContext));

            services.AddScoped<IIdentityContext, IdentityContext>();
            services.AddDbContext<IIdentityContext, IdentityContext>(options =>
            {
                options.UseSqlite(connection);
            });

            return services;
        }

        public static IApplicationBuilder UseIntegrationTestsConfiguration(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseHttpsRedirection();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseAuthorization();
            app.UseApplicationEndpoints();

            return app;
        }
    }
}
