namespace TechBlog.NewsManager.API.Middlewares
{
    public sealed class ApiKeyMiddleware
    {
        private readonly string _apiKey;
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(IConfiguration configuration, RequestDelegate next)
        {
            _apiKey = configuration["ApiKey"];
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.ToString().ToLower().Contains("swagger") &&
                context.Request.Headers["X-API-KEY"] != _apiKey)
            {
                throw new UnauthorizedAccessException("Access not permited: Invalid API-KEY");
            }

            await _next(context);
        }
    }
}
