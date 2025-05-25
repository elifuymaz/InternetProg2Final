using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace emlak.ui.Controllers
{

    public class PropertyRequestController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult MyRequests()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View(id);
        }
    }
} 