namespace LocalizationApp.Middleware
{
    public class CultureRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _defaultCulture;

        public CultureRedirectMiddleware(RequestDelegate next, string defaultCulture = "en")
        {
            _next = next;
            _defaultCulture = defaultCulture;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "/";
            var segments = path.Split('/');

            // Check if the first segment is a valid culture
            if (segments.Length >= 2 && !string.IsNullOrEmpty(segments[1]))
            {
                var culture = segments[1].ToLower();
                if (culture == "en" || culture == "de" || culture == "tr")
                {
                    await _next(context);
                    return;
                }
            }

            // If no valid culture found, redirect to default culture
            var newPath = $"/{_defaultCulture}{path}";
            context.Response.Redirect(newPath);
            return;
        }
    }

    // Extension method for easy use
    public static class CultureRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseCultureRedirect(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureRedirectMiddleware>();
        }
    }
}
