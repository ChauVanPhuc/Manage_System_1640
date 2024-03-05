using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Areas.Maketting.Controllers
{

    [Area("Maketting")]
    public class ContributionController : Controller
    {
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public ContributionController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }

        [Route("Maketting/Contributions")]
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

        [Route("/Maketting/Contributions/Publish/{id:}")]
        public IActionResult Publish(int id)
        {
            try
            {
                var contri = _db.Contributions.Find(id);
                if (contri == null)
                {
                    _notyf.Error("Contributions does not exist");
                    return Redirect("/Coordinator/Contributions");
                }
                else
                {
                    if (contri.Publics == false)
                    {
                        contri.Publics = true;
                    }
                    else
                    {
                        contri.Publics = false;
                    }

                    _db.Contributions.Update(contri);
                    _db.SaveChanges();

                    _notyf.Success("Update Success");
                    return Redirect("/Coordinator/Contributions");
                }
            }
            catch
            {

                _notyf.Error("Update Faill");
                return Redirect("/Coordinator/Contributions");
            }


        }
    }
}
