using Microsoft.AspNetCore.Mvc;

namespace Manage_System.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
