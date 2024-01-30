using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace TechBlog.NewsManager.API.Middlewares.Logger
{
    [ExcludeFromCodeCoverage]
    public sealed class LogResponseMiddleware
    {
        private readonly bool _logResponseBody;
        private readonly bool _logResponseHeaders;
        private readonly RequestDelegate _next;

        public LogResponseMiddleware(IConfiguration configuration, RequestDelegate next)
        {
            _logResponseBody = configuration.GetValue<bool>("Logging:Configuration:LogResponseBody", false);
            _logResponseHeaders = configuration.GetValue<bool>("Logging:Configuration:LogResponseHeaders", false);
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_logResponseBody && !_logResponseHeaders)
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context);

                var requestTelemetry = context.Features.Get<RequestTelemetry>();

                if (_logResponseBody)
                {
                    memoryStream.Position = 0;
                    var reader = new StreamReader(memoryStream);
                    var responseBody = await reader.ReadToEndAsync();

                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(originalBodyStream);

                    requestTelemetry?.Properties.Add("ResponseBody", responseBody);
                    requestTelemetry?.Properties.Add("ResponseBodySize", responseBody.Length.ToString());
                }

                if (_logResponseHeaders)
                {
                    var responseHeaders = JsonConvert.SerializeObject(context.Response.Headers);

                    requestTelemetry?.Properties.Add("ResponseHeaders", responseHeaders);
                    requestTelemetry?.Properties.Add("ResponseHeadersSize", responseHeaders.Length.ToString());
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}
