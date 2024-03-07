using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.ModelViews;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Controllers
{
    [Authorize]
    public class ContributionsController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly IFileService _formFile;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;

        public ContributionsController(ManageSystem1640Context db,
            IFileService formFile, INotyfService notyf, IEmailService emailService)
        {
            _db = db;
            _formFile = formFile;
            _notyf = notyf;
            _emailService = emailService;   
        }

        [Route("Student/Contributions")]
        public IActionResult Index()
        {
            var account = HttpContext.Session.GetString("AccountId");

            var contributions =  _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .Where(x => x.UserId == int.Parse(account))
                .ToList();

            return View(contributions);
        }

        [Route("Student/Contributions/Create")]
        public IActionResult Create()
        {

            ViewData["MagazineId"] = new SelectList(_db.Magazines, "Id", "Description").ToList();
            return View();
        }


        [Route("Student/Contributions/Detail/{id:}")]
        public IActionResult Detail(int id)
        {

            if (id == null || _db.Contributions == null)
            {
                return NotFound();
            }
            else
            {

                var contributions = _db.Contributions
                    .Include(x => x.ImgFiles)
                    .Include(x => x.Magazine)
                    .Include(x => x.Comments)
                    .Include(x => x.User)
                    .FirstOrDefault(b => b.Id == id);

                if (contributions == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(contributions);
                }
            }
        }

        [HttpPost]
        [Route("Student/Contributions/Create")]
        public async Task<IActionResult> Create(ContributionsModelView model)
        {
            try
            {
/*                if (ModelState.IsValid)
                {
                    
*/

                    var account = HttpContext.Session.GetString("AccountId");
                    Contribution contribution = new Contribution
                    {
                        UserId = int.Parse(account),
                        Title = model.Title,
                        ShortDescription = model.ShortDescription,
                        Content = model.Content,
                        SubmissionDate = DateTime.Now,
                        Status = false,
                        Publics = false,
                        MagazineId = model.MagazineId,
                    };

                    

                    try
                    {
                        _db.Contributions.Add(contribution);
                        _db.SaveChanges();

                        string img = null;
                        if (model.ImgFile != null)
                        {

                            foreach (var file in model.ImgFile)
                            {
                                var ext = Path.GetExtension(file.FileName);
                                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                                if (allowedExtensions.Contains(ext))
                                {
                                    var imgFile = new ImgFile
                                    {
                                        Stype = "Img",
                                        Url = _formFile.SaveImage(file),
                                        ContributionId = contribution.Id
                                    };
                                    _db.ImgFiles.Add(imgFile);
                                    
                                }
                            
                        }

                            _db.SaveChanges();

                        }

                    var user = _db.Users.Include(x => x.Role).AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));

                    var sendEmail = await (from users in _db.Users
                                      join Faculty in _db.Faculties
                                      on users.FacultyId equals Faculty.Id
                                      join role in _db.Roles
                                      on users.RoleId equals role.Id
                                      where role.Name == "Coordinator"
                                           select users).ToListAsync();

                    if (sendEmail != null)
                    {
                        foreach (var item in sendEmail)
                        {
                            await _emailService.SendEmailAsync(item.Email, "Contribution new", user.FullName + " has made a new contribution");

                        }

                    }


                    _notyf.Success("Add Contributions Success");
                        return Redirect("Student/Contributions");
                    }
                    catch
                    {
                        
                        _notyf.Error("Register Fail");
                        ViewData["MagazineId"] = new SelectList(_db.Magazines, "Id", "Description").ToList();
                        return View(model);
                    }
/*                }
                ViewData["MagazineId"] = new SelectList(_db.Magazines, "Id", "Description").ToList();
                return View(model);*/
            }
            catch (Exception)
            {
                ViewData["MagazineId"] = new SelectList(_db.Magazines, "Id", "Description").ToList();
                return View(model);
            }
            
        }
    }
}
