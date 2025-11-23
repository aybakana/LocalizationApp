namespace LocalizationApp
{
    public static class HttpContextExtensions
    {
        private static readonly string[] SupportedCultures = { "en", "de", "tr" };

        public static string ChangeCulture(this HttpContext context, string newCulture)
        {
            // Validate and default culture
            if (string.IsNullOrEmpty(newCulture) || !SupportedCultures.Contains(newCulture.ToLower()))
                newCulture = "en";

            // Get the current path
            var path = context.Request.Path.Value ?? "/";
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();

            // Remove existing culture prefix if it's the first segment
            if (segments.Count > 0 && SupportedCultures.Contains(segments[0].ToLower()))
            {
                segments.RemoveAt(0);
            }

            // Build the new path
            string pathWithoutCulture = segments.Count > 0 
                ? string.Join("/", segments) 
                : "Home"; // Default to Home controller if no path

            var newPath = $"/{newCulture}/{pathWithoutCulture}";

            // Preserve query string if present
            var queryString = context.Request.QueryString.Value ?? "";
            return newPath + queryString;
        }
    }
}