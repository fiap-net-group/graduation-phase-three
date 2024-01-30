using TechBlog.NewsManager.API.DependencyInjection;
namespace TechBlog.NewsManager.API;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
               .SetBasePath(builder.Environment.ContentRootPath)
           .AddJsonFile("appsettings.json", true, true)
           .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
           .AddEnvironmentVariables();

        var isDevelopment = builder.Environment.IsDevelopment() || IsTesting(builder.Environment.EnvironmentName);

        builder.Services.AddDependencyInjection(builder.Configuration, isDevelopment);

        var app = builder.Build();

        if (!IsTesting(builder.Environment.EnvironmentName))
            app.UseDependencyInjection(isDevelopment);

        app.Run();
    }

    public static bool IsTesting(string env)
    {
        return env == "Testing";
    }
}

