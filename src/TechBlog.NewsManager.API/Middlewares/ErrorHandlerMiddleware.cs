using FluentValidation;
using Newtonsoft.Json;
using System.Net;
using TechBlog.NewsManager.API.Domain.Exceptions;
using TechBlog.NewsManager.API.Domain.Logger;
using TechBlog.NewsManager.API.Domain.Responses;

namespace TechBlog.NewsManager.API.Middlewares
{
    public sealed class ErrorHandlerMiddleware
    {
        private readonly ILoggerManager _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(ILoggerManager logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    _logger.Log("Unauthorized error caught by middleware - JWT TOKEN", LoggerManagerSeverity.WARNING);

                    var code = HttpStatusCode.Unauthorized;

                    var result = JsonConvert.SerializeObject(new BaseResponse().AsError(ResponseMessage.UserIsNotAuthenticated));

                    await ErrorResponse(context, result, code);
                }
            }
            catch (BusinessException ex)
            {
                _logger.Log("Business error caught by middleware", LoggerManagerSeverity.WARNING, ("exception", ex));

                await HandleExceptionAsync(context, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Log("Unauthorized error caught by middleware - API TOKEN", LoggerManagerSeverity.WARNING, ("exception", ex));

                await HandleExceptionAsync(context, ex);
            }
            catch (ValidationException ex)
            {
                _logger.Log("Validation error caught by middleware", LoggerManagerSeverity.WARNING, ("exception", ex));

                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogException("Unexpected error caught by middleware", LoggerManagerSeverity.ERROR, ex);

                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, BusinessException exception)
        {
            var code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new BaseResponse().AsError(null, exception.Message));

            return ErrorResponse(context, result, code);
        }

        private static Task HandleExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
        {
            var code = HttpStatusCode.Unauthorized;

            var result = JsonConvert.SerializeObject(new BaseResponse().AsError(null, exception.Message));

            return ErrorResponse(context, result, code);
        }

        private static Task HandleExceptionAsync(HttpContext context, ValidationException exception)
        {
            var code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new BaseResponse().AsError(ResponseMessage.InvalidInformation, exception.Errors.Select(e => e.ErrorMessage).ToArray()));

            return ErrorResponse(context, result, code);
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            var code = HttpStatusCode.InternalServerError;

            return ErrorResponse(context, code);
        }

        private static Task ErrorResponse(HttpContext context, HttpStatusCode code)
        {
            var result = JsonConvert.SerializeObject(new BaseResponse().AsError());

            return ErrorResponse(context, result, code);
        }

        private static Task ErrorResponse(HttpContext context, string result, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
