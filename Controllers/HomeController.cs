using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace LocalizationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        [Route("{culture}/home")]
        public IActionResult Index()
        {
            ViewData["Title"] = _localizer["Welcome"];
            return View();
        }

        [Route("{culture}/about")]
        public IActionResult About()
        {
            ViewData["Message"] = _localizer["AboutDescription"];
            return View();
        }

        [Route("{culture}/contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = _localizer["ContactDescription"];
            return View();
        }

        [Route("{culture}/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        // Remove the RedirectToDefaultCulture action as we handle it in middleware now
    }
}