using Microsoft.AspNetCore.Mvc;

namespace Spectre.API.Controllers
{
    public class WebsiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
