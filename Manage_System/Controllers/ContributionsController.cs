using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Admin.ModelView;
using Manage_System.models;
using Manage_System.ModelViews;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Manage_System.Controllers
{
    [Authorize(Policy = "Student")]
    public class ContributionsController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly IFileService _formFile;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public ContributionsController(ManageSystem1640Context db,
            IFileService formFile, INotyfService notyf, IEmailService emailService, IWebHostEnvironment env)
        {
            _db = db;
            _formFile = formFile;
            _notyf = notyf;
            _emailService = emailService; 
            _env = env;
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
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(contributions);
        }

        [Route("Student/Contributions/Create")]
        public IActionResult Create()
        {
            var currentYear = DateTime.Now.Year;
            ViewData["MagazineId"] = new SelectList(_db.Magazines.Where(m =>m.ClosureDay.Value.Year == currentYear), "Id", "Description").ToList();
            return View();
        }


        [Route("Student/Contributions/Detail/{id:}")]
        public async Task<IActionResult> Detail(int id)
        {

            if (id == null || _db.Contributions == null)
            {
                return Redirect("/Student/Contributions");
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
                var user = _db.Users.Find( int.Parse(account));

                

                if (contributions != null)
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
                        Comments = comments,
                        coordinatorId = GetCoordinatorId()
                    };
                    return View(model);
                }


                return NotFound();

            }
        }

        public int GetCoordinatorId()
        {
            var coordinatorId = (from users in _db.Users
                                 join Faculty in _db.Faculties
                                 on users.FacultyId equals Faculty.Id
                                 join role in _db.Roles
                                 on users.RoleId equals role.Id
                                 where role.Name == "Coordinator"
                                 select users).FirstOrDefault();

            return coordinatorId.Id;
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
                        SubmissionDate = DateTime.Now,
                        Status = "Processing",
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
                                    _db.Contributions.Remove(contribution);
                                    _db.SaveChanges();

                                    var currentYear1 = DateTime.Now.Year;
                                    ViewData["MagazineId"] = new SelectList(_db.Magazines.Where(m => m.ClosureDay.Value.Year == currentYear1), "Id", "Description").ToList();
                                    _notyf.Error("Only accepts Images or files 'doc' and '.docx' ");
                                    ViewData["Error"] = "Only accepts Images or files 'doc' and '.docx' ";
                                    return View(model);
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
                                      where role.Name == "Coordinator" && Faculty.Id == user.FacultyId
                                           select users).ToList();

                    if (sendEmail != null)
                    {
                        foreach (var item in sendEmail)
                        {
                            await _emailService.SendEmailAsync(item.Email, "Contribution new", user.FullName + " has made a new contribution");

                        }

                    }


                        _notyf.Success("Add Contributions Success");
                        return RedirectToAction("Index");
                    
                }
                var currentYear = DateTime.Now.Year;
                ViewData["MagazineId"] = new SelectList(_db.Magazines.Where(m => m.ClosureDay.Value.Year == currentYear), "Id", "Description").ToList();
                _notyf.Error("Add Contributions Faill");
                return View(model);
            }
            catch (Exception)
            {
                var currentYear = DateTime.Now.Year;
                ViewData["MagazineId"] = new SelectList(_db.Magazines.Where(m => m.ClosureDay.Value.Year == currentYear), "Id", "Description").ToList();

                _notyf.Error("Add Contributions Faill");
                return View(model);
            }

        }


        [Route("/Student/Contributions/Edit/{id:}")]
        public IActionResult Edit(int id)
        {

            var contributions = _db.Contributions
                    .Include(x => x.ImgFiles)
                    .Include(x => x.Magazine)
                    .Include(x => x.Comments)
                    .Include(x => x.User)
                    .FirstOrDefault(b => b.Id == id);

            if (contributions == null)
            {
                _notyf.Error("Contributions not exist");
                return RedirectToAction("Index");
            }

            ContributionsModelEdit model = new ContributionsModelEdit
            {
                Id = contributions.Id,
                User = contributions.User,
                Title = contributions.Title,
                SubmissionDate = contributions.SubmissionDate,
                LastModifiedDate = contributions.LastModifiedDate,
                Status = contributions.Status,
                Publics = contributions.Publics,
                Magazine = contributions.Magazine,
                ShortDescription = contributions.ShortDescription,
                ImgFiles = contributions.ImgFiles,
            };

            var currentYear = DateTime.Now.Year;
            ViewData["MagazineId"] = new SelectList(_db.Magazines.Where(m => m.ClosureDay.Value.Year == currentYear), "Id", "Description").ToList();
            return View(model);
        }


        [HttpPost]
        [Route("/Student/Contributions/Edit/{id:}")]
        public IActionResult Edit(ContributionsModelEdit model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contributions = _db.Contributions
                    .Include(x => x.ImgFiles)
                    .Include(x => x.Magazine)
                    .Include(x => x.Comments)
                    .Include(x => x.User)
                    .FirstOrDefault(b => b.Id == model.Id);

                    contributions.Title = model.Title;
                    contributions.LastModifiedDate = DateTime.Now;
                    contributions.ShortDescription = model.ShortDescription;


                    string img = null;
                    if (model.ImgFile != null)
                    {
                        foreach (var file in model.ImgFile)
                        {
                            var ext = Path.GetExtension(file.FileName);
                            var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                            var allowedFile = new string[] { ".doc", ".docx" };
                            if (allowedExtensions.Contains(ext))
                            {
                                var imgFile = new ImgFile
                                {
                                    Stype = "Img",
                                    Url = _formFile.SaveImage(file),
                                    ContributionId = model.Id
                                };
                                _db.ImgFiles.Add(imgFile);

                            }
                            else if (allowedFile.Contains(ext))
                            {
                                var docfile = new ImgFile
                                {
                                    Stype = "File",
                                    Url = _formFile.SaveImage(file),
                                    ContributionId = model.Id
                                };
                                _db.ImgFiles.Add(docfile);

                            }
                            else
                            {
                                _notyf.Error("Only accepts Images or files 'doc' and '.docx' ");
                            }
                        }
                    }


                    _db.Contributions.Update(contributions);
                    _db.SaveChanges();

                    _notyf.Success("Update Contributions Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    var currentYear = DateTime.Now.Year;
                    ViewData["MagazineId"] = new SelectList(_db.Magazines.Where(m => m.ClosureDay.Value.Year == currentYear), "Id", "Description").ToList();
                    _notyf.Success("Update Contributions Fail");
                    return View(model);
                }
            }
            else
            {
                var currentYear = DateTime.Now.Year;
                ViewData["MagazineId"] = new SelectList(_db.Magazines.Where(m => m.ClosureDay.Value.Year == currentYear), "Id", "Description").ToList();
                _notyf.Success("Update Contributions Fail");
                return View(model);
            }
        }

        [Route("/Student/Contributions/DeleteFile")]
        public ActionResult DeleteFile(string UrlFile, int contriId, int imgId)
        {
            _formFile.Delete(UrlFile);

            var file = _db.ImgFiles.Find(imgId);
            if (file != null)
            {
                _db.ImgFiles.Remove(file);
                _db.SaveChanges();
            }

            return Redirect("/Student/Contributions/Edit/"+ contriId + "");
        }


        [Route("/Student/Contributions/DownloadFile/{id:}")]
        public IActionResult DownloadFile(int id)
        {
            var urlFile = _db.ImgFiles
                .Include(x => x.Contribution)
                .ThenInclude(x => x.User)
                .Where(x => x.ContributionId == id)
                .ToList();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                string userName = "";

                using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in urlFile)
                    {
                        // Đường dẫn đầy đủ của tệp trong tệp Zip
                        var wwwPath = this._env.WebRootPath;
                        var entryPath = Path.Combine(wwwPath, "Uploads\\", file.Url);


                        var fileInfor = new FileInfo(entryPath);

                        // Tạo entry cho tệp        
                        var entry = archive.CreateEntry(file.Url);


                        using (var entryStream = entry.Open())
                        using (var fileStream = new FileStream(entryPath, FileMode.Open, FileAccess.Read))
                        {
                            fileStream.CopyTo(entryStream);
                        }

                        userName = (file.Contribution.User.FullName + "_" + file.Contribution.Title).Replace(" ", "_");
                    }

                }

                
                return File(memoryStream.ToArray(), "application/zip", "" + userName.ToString() + ".zip");

            }


        }

    }
}
