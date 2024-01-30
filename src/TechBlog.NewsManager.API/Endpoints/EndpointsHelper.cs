namespace TechBlog.NewsManager.API.Endpoints
{
    public static class EndpointsHelper
    {
        public static IApplicationBuilder UseApplicationEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlogNewsEndpoints();
                endpoints.MapBlogUsersEndpoints();
                endpoints.MapAuthenticationEndpoints();
            });

            return app;
        }
    }
}
