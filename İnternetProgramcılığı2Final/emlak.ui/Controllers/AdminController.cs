using Microsoft.AspNetCore.Mvc;

namespace emlak.ui.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Properties()
        {
            return View();
        }

        public IActionResult PendingProperties()
        {
            return View();
        }

        public IActionResult RejecetProperties()
        {
            return View();
        }

        public IActionResult Cities()
        {
            return View();
        }

        public IActionResult Districts()
        {
            return View();
        }

      
    }
}
