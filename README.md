# LocalizationApp

This project is an ASP.NET Core web application demonstrating how to implement localization to display web pages in multiple languages based on the URL.

## Key Features

*   **Route-Based Localization**: The application determines the display language from a culture segment in the URL (e.g., `/en`, `/de`, `/tr`).
*   **Multiple Language Support**: Supports English (en), German (de), and Turkish (tr).
*   **Localized Resources**: Uses `.resx` files to store localized strings for controllers, views, and shared components.
*   **Automatic Redirection**: Automatically redirects root URLs and culture-less URLs to the default English culture.

## Supported Cultures

*   **English (en)**: Default culture.
*   **German (de)**
*   **Turkish (tr)**

## How Localization Works

The localization in this application is implemented using ASP.NET Core's built-in localization features, primarily through resource files and route-based culture detection.

### 1. Resource Files (`.resx`)

Localized strings are stored in XML-based `.resx` files within the `Resources` directory. These files are organized hierarchically:

*   **Controller-specific resources**: Located in `Resources/Controllers/` (e.g., `HomeController.en.resx`, `HomeController.de.resx`, `HomeController.tr.resx`). These provide strings used by specific controllers.
*   **View-specific resources**: Located in `Resources/Views/` (e.g., `Resources/Views/Home/Index.de.resx`, `Resources/Views/Shared/_Layout.en.resx`). These provide strings used within Razor views.

Each `.resx` file contains key-value pairs, where the key identifies the string and the value is its translation for the specific culture.

### 2. Application Configuration (`Program.cs`)

The `Program.cs` file is central to setting up the localization pipeline:

*   **Service Registration**:
    *   `builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");` registers localization services and specifies "Resources" as the base directory for `.resx` files.
    *   `AddViewLocalization()` enables localization for Razor views, allowing them to inject `IViewLocalizer`.
    *   `AddDataAnnotationsLocalization()` enables localization for data annotation attributes (e.g., validation messages).
*   **Culture Configuration**:
    *   `RequestLocalizationOptions` are configured to define the `DefaultRequestCulture` as "en" and list `en`, `de`, `tr` as `SupportedCultures` and `SupportedUICultures`.
    *   `RouteDataRequestCultureProvider` is prioritized, meaning the application will first attempt to determine the culture from the URL route.
*   **Middleware**:
    *   `app.UseRequestLocalization(localizationOptions);` applies the configured localization settings to incoming requests.
*   **Routing**:
    *   The application's routing is configured to include a `{culture}` segment:
        ```csharp
        app.MapControllerRoute(
            name: "default",
            pattern: "{culture:regex(" + culturePattern + ")}/{controller=Home}/{action=Index}/{id?}");
        ```
    *   This ensures that URLs like `/en/Home/Index`, `/de/Home/About`, or `/tr/Home/Contact` are correctly routed and the culture is extracted.
*   **Redirection for Culture-less URLs**:
    *   `app.MapGet("/", ...)` and `app.MapGet("/{controller=Home}/{action=Index}/{id?}", ...)` are used to automatically redirect requests without a culture segment (e.g., `/` or `/Home/Index`) to the default English culture (e.g., `/en/Home`).

### 3. Controller Usage (`Controllers/HomeController.cs`)

Controllers inject `IStringLocalizer<T>` (e.g., `IStringLocalizer<HomeController>`) to retrieve localized strings. For example:

```csharp
public class HomeController : Controller
{
    private readonly IStringLocalizer<HomeController> _localizer;

    public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer)
    {
        _localizer = localizer;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = _localizer["Welcome"]; // Retrieves "Welcome" from HomeController.resx
        return View();
    }
}
```

### 4. View Usage (`Views/Home/Index.cshtml`, `Views/Shared/_Layout.cshtml`)

Razor views inject `IViewLocalizer` to display localized content directly within the HTML. For example:

```html
@inject IViewLocalizer Localizer

<h1 class="display-4">@Localizer["Welcome"]</h1>
<p>Current Culture: @System.Globalization.CultureInfo.CurrentCulture.Name</p>
```

### Note on `CultureRedirectMiddleware`

The project includes a `Middleware/CultureRedirectMiddleware.cs` file. While present, its functionality for redirecting culture-less URLs is currently handled directly by the `app.MapGet` configurations in `Program.cs`, making the middleware redundant in the current setup.

## How to Run the Application

To run this ASP.NET Core application:

1.  **Clone the repository** (if you haven't already).
2.  **Navigate to the project directory**: `cd LocalizationApp`
3.  **Restore dependencies**: `dotnet restore`
4.  **Run the application**: `dotnet run`
5.  **Access in browser**: Open your web browser and navigate to `https://localhost:7000` (or the port specified in your `launchSettings.json`). The application will redirect to `/en/Home`. You can then manually change the URL to `/de/Home` or `/tr/Home` to see the localization in action.
