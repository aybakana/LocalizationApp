public static class UrlExtensions
{
    public static string ChangeCulture(this HttpContext context, string newCulture)
    {
        var path = context.Request.Path.Value ?? "/";
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var supportedCultures = new[] { "en", "de", "tr" };

        // Remove existing culture if present
        if (segments.Length > 0 && supportedCultures.Contains(segments[0]))
        {
            segments = segments.Skip(1).ToArray();
        }

        // Build new path with new culture
        var newPath = "/" + newCulture + "/" + string.Join("/", segments);
        return newPath.TrimEnd('/') == $"/{newCulture}" ? $"/{newCulture}/home" : newPath;
    }
}