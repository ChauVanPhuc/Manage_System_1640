using Manage_System.models;
using Manage_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Manage_System.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ManageSystem1640Context _db;

        public HomeController(ILogger<HomeController> logger, ManageSystem1640Context db)
        {
            _logger = logger;
            _db = db;
        }

        
        public IActionResult Index()
        {
            var account = HttpContext.Session.GetString("AccountId");
            var tk = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(account));
            if (account != null)
            {
                return View();
            }
            return Redirect("/Login");
            
        }

        public IActionResult Privacy()
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