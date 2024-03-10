using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Coordinator.ModelView;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    [Authorize]
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
        public async Task<IActionResult> Index()
        {
            var account = HttpContext.Session.GetString("AccountId");

            var contributions = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .ThenInclude(x => x.Faculty)
                .Where(x => x.User.FacultyId == x.User.Faculty.Id)
                .ToList();


            return View(contributions);
        }





        [Route("Coordinator/Contributions/Detail/{id:}")]
        public IActionResult Detail(int id)
        {

            if (id == null || _db.Contributions == null)
            {
                return Redirect("/Coordinator/Contributions");
            }
            else
            {

                var contributions = _db.Contributions
                    .Include(x => x.ImgFiles)
                    .Include(x => x.Magazine)
                    .Include(x => x.Comments)
                    .Include(x => x.User)
                    .FirstOrDefault(b => b.Id == id);

                List<Comment> comments = _db.Comments.Include(x => x.User).Where(x => x.ContributionId == id).ToList();

                var account = HttpContext.Session.GetString("AccountId");
                var user = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));

                

                if (contributions!=null)
                {
                    ContributionsModelView model = new ContributionsModelView
                    {
                        Id = contributions.Id,
                        User = contributions.User,
                        Coordinator = user,
                        Title = contributions.Title,
                        SubmissionDate = contributions.SubmissionDate,
                        LastModifiedDate = contributions.LastModifiedDate,
                        Status = contributions.Status,
                        Publics = contributions.Publics,
                        Magazine = contributions.Magazine,
                        ShortDescription = contributions.ShortDescription,
                        ImgFiles = contributions.ImgFiles,
                        Comments = comments
                    };
                    return View(model);
                }


                return NotFound();

            }
        }

        [Route("/Coordinator/Contributions/Allow/{id:}")]
        public IActionResult Allow(int id)
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
                    if (contri.Status == false)
                    {
                        contri.Status = true;
                    }
                    else
                    {
                        contri.Status = false;
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
