using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TechBlog.NewsManager.API.Middlewares.Logger
{
    [ExcludeFromCodeCoverage]
    public sealed class LogRequestMiddleware
    {
        private readonly bool _logRequestBody;
        private readonly bool _logRequestHeaders;
        private readonly RequestDelegate _next;

        public LogRequestMiddleware(IConfiguration configuration, RequestDelegate next)
        {
            _logRequestBody = configuration.GetValue<bool>("Logging:Configuration:LogRequestBody", false);
            _logRequestHeaders = configuration.GetValue<bool>("Logging:Configuration:LogRequestHeaders", false);
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_logRequestBody && !_logRequestHeaders)
            {
                await _next(context);
                return;
            }

            var requestTelemetry = context.Features.Get<RequestTelemetry>();

            if (_logRequestBody)
            {
                var method = context.Request.Method;

                context.Request.EnableBuffering();

                if (context.Request.Body.CanRead && (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsPatch(method)))
                {
                    using var reader = new StreamReader(
                        context.Request.Body,
                        Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        bufferSize: 512, leaveOpen: true);

                    var requestBody = await reader.ReadToEndAsync();

                    context.Request.Body.Position = 0;

                    requestTelemetry?.Properties.Add("RequestBody", requestBody);
                    requestTelemetry?.Properties.Add("RequestBodySize", requestBody.Length.ToString());
                }
            }

            if (_logRequestHeaders)
            {
                var requestHeaders = JsonConvert.SerializeObject(context.Request.Headers);

                requestTelemetry?.Properties.Add("RequestHeaders", requestHeaders);
                requestTelemetry?.Properties.Add("RequestHeadersSize", requestHeaders.Length.ToString());
            }

            await _next(context);
        }
    }
}
