using System.Diagnostics;
using emlak.ui.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace emlak.ui.Controllers
{
 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult MyProperties()
        {
            return View();
        }

        public IActionResult PropertyApplication()
        {
            return View();
        }

        public IActionResult PropertyDetails(int id)
        {
            ViewBag.PropertyId = id;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GetApprovedProperties()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
