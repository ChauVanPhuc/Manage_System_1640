using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    public class ContributionController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly IFileService _formFile;
        private readonly INotyfService _notyf;

        public ContributionController(ManageSystem1640Context db, IFileService formFile, INotyfService notyf)
        {
            _db = db;
            _formFile = formFile;
            _notyf = notyf;
        }

        [Route("Coordinator/Contributions")]
        public IActionResult Index()
        {
            var contributions = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .ToList();

            return View(contributions);
        }
    }
}
