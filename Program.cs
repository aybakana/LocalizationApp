using LocalizationApp.Middleware;
using LocalizationApp.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

// Configure supported cultures
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("de"),
    new CultureInfo("tr")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Prioritize RouteDataRequestCultureProvider to ensure URL culture is used
    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        new RouteDataRequestCultureProvider(),
        //new QueryStringRequestCultureProvider(),
        //new CookieRequestCultureProvider(),
        //new AcceptLanguageHeaderRequestCultureProvider()
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Apply request localization AFTER routing to access route data
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

// Configure routes with culture constraint
var culturePattern = $"^({string.Join("|", supportedCultures.Select(c => c.Name))})$";

app.MapControllerRoute(
    name: "default",
    pattern: "{culture:regex(" + culturePattern + ")}/{controller=Home}/{action=Index}/{id?}");

// Redirect root to default culture
app.MapGet("/", context =>
{
    var defaultCulture = "en"; // or get from user preferences/browser
    context.Response.Redirect($"/{defaultCulture}/Home");
    return Task.CompletedTask;
});

// Optional: Handle culture-less URLs by redirecting to default culture
app.MapGet("/{controller=Home}/{action=Index}/{id?}", context =>
{
    var defaultCulture = "en";
    var path = context.Request.Path;
    context.Response.Redirect($"/{defaultCulture}{path}");
    return Task.CompletedTask;
});

app.Run();