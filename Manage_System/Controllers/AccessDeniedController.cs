using Microsoft.AspNetCore.Mvc;

namespace Manage_System.Controllers
{
    public class AccessDeniedController : Controller
    {
        [Route("/AccessDenied")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
