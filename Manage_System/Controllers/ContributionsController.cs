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

                List<Comment> comments = _db.Comments
                    .Include(x => x.User)
                    .Where(x => x.ContributionId == id)
                    .OrderByDescending(x => x.Id)
                    .ToList();

                var account = HttpContext.Session.GetString("AccountId");
                var user = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));



                if (contributions != null)
                {
                    ContributionsModelView model = new ContributionsModelView
                    {
                        Id = contributions.Id,
                        User = contributions.User,
                        Coordinator = user,
                        Title = contributions.Title,
                        Content = contributions.Content,
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

        [HttpPost]
        [Route("Student/Contributions/Create")]
        public async Task<IActionResult> Create(ContributionsModelView model)
        {
            try
            {
                if (ModelState.IsValid)
                {


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

                   
                        _db.Contributions.Add(contribution);
                        _db.SaveChanges();

                        string img = null;
                        if (model.ImgFile != null)
                        {
                            foreach (var file in model.ImgFile)
                            {
                                var ext = Path.GetExtension(file.FileName);
                                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                                var allowedFile = new string[] { ".doc",".docx" };
                                if (allowedExtensions.Contains(ext))
                                {
                                    var imgFile = new ImgFile
                                    {
                                        Stype = "Img",
                                        Url = _formFile.SaveImage(file),
                                        ContributionId = contribution.Id
                                    };
                                    _db.ImgFiles.Add(imgFile);
                                    
                                }else if (allowedFile.Contains(ext))
                                {
                                    var docfile = new ImgFile
                                    {
                                        Stype = "File",
                                        Url = _formFile.SaveImage(file),
                                        ContributionId = contribution.Id
                                    };
                                    _db.ImgFiles.Add(docfile);

                                }else
                                {
                                    _notyf.Error("Only accepts Images or files 'doc' and '.docx' ");
                                }



                        }

                            _db.SaveChanges();
                        }

                    var user = _db.Users.Include(x => x.Role).AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));

                    var sendEmail = (from users in _db.Users
                                      join Faculty in _db.Faculties
                                      on users.FacultyId equals Faculty.Id
                                      join role in _db.Roles
                                      on users.RoleId equals role.Id
                                      where role.Name == "Coordinator"
                                           select users).ToList();

                    if (sendEmail != null)
                    {
                        foreach (var item in sendEmail)
                        {
                            _emailService.SendEmailAsync(item.Email, "Contribution new", user.FullName + " has made a new contribution");

                        }

                    }


                        _notyf.Success("Add Contributions Success");
                        return RedirectToAction("Index");
                    
                }
                ViewData["MagazineId"] = new SelectList(_db.Magazines, "Id", "Description").ToList();
                _notyf.Error("Add Contributions Faill");
                return View(model);
            }
            catch (Exception)
            {
                ViewData["MagazineId"] = new SelectList(_db.Magazines, "Id", "Description").ToList();
                _notyf.Error("Add Contributions Faill");
                return View(model);
            }

        }
    }
}
