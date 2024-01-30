using FluentValidation;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using TechBlog.NewsManager.API.Domain.Logger;

namespace TechBlog.NewsManager.API.Infrastructure.Logger.ApplicationInsights
{
    [ExcludeFromCodeCoverage]
    public class ApplicationInsightsLogger : ILoggerManager
    {
        private readonly LoggerManagerSeverity _minLevel;
        private readonly TelemetryClient _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApplicationInsightsLogger(IConfiguration configuration, TelemetryClient logger, IHttpContextAccessor contextAccessor)
        {
            if (!Enum.TryParse(configuration.GetValue("Logging:LogLevel:Default", LogLevel.Debug.ToString()).ToUpper(), out _minLevel))
                throw new ArgumentException("Invalid log level");

            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public void Log(string message, LoggerManagerSeverity severity)
        {
            if (_minLevel > severity)
                return;

            Log(message, severity, Array.Empty<(string, string)>());
        }

        public void Log(string message, LoggerManagerSeverity severity, params (string name, object value)[] parameters)
        {
            if (_minLevel >= severity)
                return;

            parameters ??= Array.Empty<(string, object)>();
            var parametersAsString = new (string name, string value)[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                parametersAsString[i] = (parameters[i].name, JsonConvert.SerializeObject(parameters[i].value));

            Log(message, severity, parametersAsString);
        }

        public void Log(string message, LoggerManagerSeverity severity, params (string name, string value)[] parameters)
        {
            if (_minLevel > severity)
                return;

            parameters ??= Array.Empty<(string, string)>();

            var traceTelemetry = GenerateTraceTelemetry(message, severity);

            for (int i = 0; i < parameters.Length; i++)
                traceTelemetry.Properties.Add(parameters[i].name, parameters[i].value);

            _logger.TrackTrace(traceTelemetry);
        }

        private TraceTelemetry GenerateTraceTelemetry(string message, LoggerManagerSeverity severity)
        {
            var traceTelemetry = new TraceTelemetry
            {
                Message = message,
                SeverityLevel = GetSeverityLevel(severity),
            };

            var currentRequest = _contextAccessor.HttpContext.Features.Get<RequestTelemetry>();

            if (currentRequest != null)
            {
                traceTelemetry.Context.Operation.Id = currentRequest.Context.Operation.Id;
                traceTelemetry.Context.Operation.ParentId = currentRequest.Context.Operation.ParentId;
            }

            return traceTelemetry;
        }

        public void LogException(string message, LoggerManagerSeverity severity, Exception exception = default)
        {
            if (_minLevel > severity)
                return;

            var parameters = Array.Empty<(string, string)>();

            LogException(message, severity, exception, parameters);
        }

        public void LogException(string message, LoggerManagerSeverity severity, Exception exception = default, params (string name, object value)[] parameters)
        {
            if (_minLevel > severity)
                return;

            parameters ??= Array.Empty<(string, object)>();
            var parametersAsString = new (string name, string value)[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                parametersAsString[i] = (parameters[i].name, JsonConvert.SerializeObject(parameters[i].value));

            LogException(message, severity, exception, parametersAsString);
        }

        public void LogException(string message, LoggerManagerSeverity severity, Exception exception = default, params (string name, string value)[] parameters)
        {
            if (_minLevel >= severity)
                return;

            parameters ??= Array.Empty<(string, string)>();

            Log(message, severity, parameters);

            if (exception == null)
                return;

            var exceptionTelemetry = GenerateExceptionTelemetry(exception);

            for (int i = 0; i < parameters.Length; i++)
                exceptionTelemetry.Properties.Add(parameters[i].name, JsonConvert.SerializeObject(parameters[i].value));

            _logger.TrackException(exceptionTelemetry);
        }

        private ExceptionTelemetry GenerateExceptionTelemetry(Exception exception)
        {
            var exceptionTelemetry = new ExceptionTelemetry
            {
                Message = exception.Message,
                Exception = exception,
            };

            var currentRequest = _contextAccessor.HttpContext.Features.Get<RequestTelemetry>();

            if (currentRequest != null)
            {
                exceptionTelemetry.Context.Operation.Id = currentRequest.Context.Operation.Id;
                exceptionTelemetry.Context.Operation.ParentId = currentRequest.Context.Operation.ParentId;
            }

            return exceptionTelemetry;
        }

        private static SeverityLevel GetSeverityLevel(LoggerManagerSeverity severity)
        {
            return severity switch
            {
                LoggerManagerSeverity.INFORMATION => SeverityLevel.Information,
                LoggerManagerSeverity.WARNING => SeverityLevel.Warning,
                LoggerManagerSeverity.ERROR => SeverityLevel.Error,
                LoggerManagerSeverity.CRITICAL => SeverityLevel.Critical,
                _ => SeverityLevel.Verbose,
            };
        }
    }
}
