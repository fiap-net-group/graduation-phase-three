using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics.CodeAnalysis;

namespace TechBlog.NewsManager.API.Middlewares.Logger
{
    [ExcludeFromCodeCoverage]
    public sealed class LoggerMiddleware
    {
        private readonly TelemetryClient _logger;
        private readonly RequestDelegate _next;

        public LoggerMiddleware(TelemetryClient logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestTelemetry = new RequestTelemetry
            {
                Name = $"{context.Request.Method} {context.Request.Path}"
            };

            if (context.Request.Headers.ContainsKey("Request-Id"))
            {
                var requestId = context.Request.Headers["Request-Id"];
                requestTelemetry.Context.Operation.Id = GetOperationId(requestId);
                requestTelemetry.Context.Operation.ParentId = requestId;
            }
            else
                requestTelemetry.GenerateOperationId();

            var operation = _logger.StartOperation(requestTelemetry);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                requestTelemetry.Success = false;
                _logger.TrackException(ex);
            }
            finally
            {
                if (context.Response != null)
                {
                    requestTelemetry.ResponseCode = context.Response.StatusCode.ToString();
                    requestTelemetry.Success = context.Response.StatusCode >= 200 && context.Response.StatusCode <= 299;
                }
                else
                {
                    requestTelemetry.Success = false;
                }

                _logger.StopOperation(operation);
            }
        }

        public static string GetOperationId(string id)
        {
            // Returns the root ID from the '|' to the first '.' if any.
            int rootEnd = id.IndexOf('.');
            if (rootEnd < 0)
                rootEnd = id.Length;

            int rootStart = id[0] == '|' ? 1 : 0;
            return id.Substring(rootStart, rootEnd - rootStart);
        }
    }
}
