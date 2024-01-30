using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace TechBlog.NewsManager.API.Infrastructure.Logger.ApplicationInsights
{
    public class CloudRoleNameInitializer : ITelemetryInitializer
    {
        private readonly string _appName;

        public CloudRoleNameInitializer(IConfiguration configuration)
        {
            _appName = configuration["Logging:Configuration:ApplicationName"];

            ArgumentException.ThrowIfNullOrEmpty(_appName);
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _appName;
        }
    }
}
