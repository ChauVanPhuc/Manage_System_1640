﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.Models;
using Manage_System.ModelViews;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;

namespace Manage_System.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;
        IWebHostEnvironment _env;
        private readonly IEmailService _email;

        public HomeController(ILogger<HomeController> logger,
            ManageSystem1640Context db, INotyfService notyf, IWebHostEnvironment env, IEmailService email)
        {
            _logger = logger;
            _db = db;
            _notyf = notyf;
            _env = env;
            _email = email;
        }


        [Route("/Student/Person")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> Person()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account == null)
            {
                return Redirect("/Login");
            }

            var user = _db.Users.Find(int.Parse(account));

            var allMessages = await _db.Messages.Where(x =>
                x.Sender == user.Id ||
                x.Receiver == user.Id)
                .ToListAsync();

            var chats = new List<ChatViewModel>();
            var users = await _db.Users.Include(x => x.Role).Include(x => x.Faculty).Where(x => x.RoleId == x.Role.Id && x.FacultyId == user.FacultyId && x.Role.Name == "Student" || x.Role.Name == "Coordinator" && x.FacultyId == user.FacultyId).ToListAsync();
            foreach (var i in users)
            {
                if (i != user)
                {

                    var chat = new ChatViewModel()
                    {
                        MyMessages = allMessages.Where(x => x.Sender == user.Id && x.Receiver == i.Id).ToList(),
                        OtherMessages = allMessages.Where(x => x.Sender == i.Id && x.Receiver == user.Id).ToList(),
                        RecipientName = i.FullName,
                        revId = i.Id,
                        sendvId = user.Id,
                        roleName = i.Role.Name
                    };

                    

                    var chatMessages = new List<Message>();
                    chatMessages.AddRange(chat.MyMessages);
                    chatMessages.AddRange(chat.OtherMessages);

                    chat.LastMessage = chatMessages.OrderByDescending(x => x.SentAt).FirstOrDefault();

                    chats.Add(chat);
                }
            }

            return View(chats);
        }

        [Route("/Student/Chat/{id:}")]
        [Authorize(Policy = "Student")]
        public async Task<IActionResult> Chat(int id)
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account == null)
            {
                return Redirect("/Login");
            }

            var user = _db.Users.Find(int.Parse(account));

            var allMessages = await _db.Messages.Where(x =>
                x.Sender == user.Id ||
                x.Receiver == user.Id)
                .ToListAsync();

            var chats = new List<ChatViewModel>();

            var rev = _db.Users.Where(x => x.Id == user.Id || x.Id == id).ToList();

            foreach (var i in rev)
            {
                if (i != user)
                {

                    var chat = new ChatViewModel()
                    {
                        MyMessages = allMessages.Where(x => x.Sender == user.Id && x.Receiver == i.Id).ToList(),
                        OtherMessages = allMessages.Where(x => x.Sender == i.Id && x.Receiver == user.Id).ToList(),
                        RecipientName = i.FullName,
                        revId = i.Id,
                        sendvId = user.Id
                    };

                    var chatMessages = new List<Message>();
                    chatMessages.AddRange(chat.MyMessages);
                    chatMessages.AddRange(chat.OtherMessages);

                    chat.LastMessage = chatMessages.OrderByDescending(x => x.SentAt).FirstOrDefault();

                    chats.Add(chat);
                }
            }

            return View(chats);
        }

        public IActionResult Index()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {
                var role = _db.Users.Include(x => x.Role).AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));

                return Redirect("/" + role.Role.Name + "");
            }
            return Redirect("/Login");

        }

        [Route("/Guest")]
        public IActionResult Guest(int magazineId = 0)
        {
            var account = HttpContext.Session.GetString("AccountId");
            var user = _db.Users.Include(x => x.Faculty).AsNoTracking().FirstOrDefault(x => x.Id == int.Parse(account));
            if (account != null)
            {
                int faculty = user.Faculty.Id;  
                IEnumerable<Contribution> contributions = GetContributions(magazineId, faculty);
                IEnumerable<Magazine> magazines =  _db.Magazines.ToList() ;
                GuesModelDisplay guesModelDisplay = new GuesModelDisplay
                {
                    Contributions = contributions,
                    Magazines = magazines
                };

                return View(guesModelDisplay);
            }
            return Redirect("/Login");

        }


        [Route("/Guest/Statis")]
        public IActionResult StatisGuest(int magazineId = 0)
        {
            var account = HttpContext.Session.GetString("AccountId");

            if (account != null)
            {
                var user = _db.Users.Include(x => x.Faculty).AsNoTracking().FirstOrDefault(x => x.Id == int.Parse(account));

                IEnumerable<Contribution> contribution = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .Where(x => x.Publics == true && x.User.FacultyId == user.FacultyId)
                .ToList();

                IEnumerable<Contribution> contributions = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User )
                .Where(x => x.User.FacultyId == user.FacultyId)
                .ToList();

                IEnumerable<Magazine> magazines = _db.Magazines.ToList();
                GuesModelDisplay guesModelDisplay = new GuesModelDisplay
                {
                    Magazines = magazines
                };

                var faculty = _db.Faculties.Include(c => c.Users).ThenInclude(x => x.Contributions).ToList();
              
                var lable = faculty.Select(x => x.Name).ToList();
                var contriApproved = contributions.Where(x => x.Status == "Approved").Count();
                var contriReject = contributions.Where(x => x.Status == "Reject").Count();

                var facultyContributionCounts = faculty.Select(f => f.Users
                                                        .SelectMany(u => u.Contributions)
                                                        .Where(x => x.Status == "Approved")
                                                        .Count())
                                                        .ToList();

                if (magazineId != 0)
                {
                    facultyContributionCounts = faculty.Select(f => f.Users
                                                     .SelectMany(u => u.Contributions)
                                                     .Where(x => x.Status == "Approved" && x.MagazineId == magazineId)
                                                     .Count())
                                                     .ToList();

                    contriApproved = contributions.Where(x => x.Status == "Approved" && x.MagazineId == magazineId && x.User.FacultyId == user.FacultyId).Count();
                    contriReject = contributions.Where(x => x.Status == "Reject" && x.MagazineId == magazineId && x.User.FacultyId == user.FacultyId).Count();
                }

                ViewBag.facultyCounts = facultyContributionCounts;
                ViewBag.facultyName = lable;


                ViewBag.contriApproved = contriApproved;
                ViewBag.contriReject = contriReject;

                return View(guesModelDisplay);
            }
            return Redirect("/Login");

        }

        private IEnumerable<Contribution> GetContributions(int magazineId, int facultyId)
        {

            IEnumerable<Contribution> contribution = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .Where(x => x.Publics == true && x.User.FacultyId == facultyId)
                .ToList();



            if (magazineId != 0)
            {
                contribution = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .Where(x => x.Publics == true && x.MagazineId == magazineId && x.User.FacultyId == facultyId)
                .ToList();
                
            }

            return contribution;
        }

        [Route("Guest/Contributions/Detail/{id:}")]

        public IActionResult ContriDetail(int id)
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


        [Route("/Admin")]
        [Authorize(Policy = "Admin")]
        public IActionResult Admin()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {

                var role =  _db.Roles.Include(c => c.Users).ToList();

                var roleName = role.Select(c => c.Name).ToList();
                var accountCounts = role.Select(c => c.Users.Count).ToList();

                ViewBag.roleName = roleName;
                ViewBag.accountCounts = accountCounts;


                var faculty = _db.Faculties.Include(c => c.Users).ToList();
                var facultyName = faculty.Select(c => c.Name).ToList();
                var facultyCounts = faculty.Select(c => c.Users.Count).ToList();

                ViewBag.facultyName = facultyName;
                ViewBag.facultyCounts = facultyCounts;
                return View();
            }
            return Redirect("/Login");

        }

        [Route("/Coordinator")]
        [Authorize(Policy = "Coordinator")]
        public IActionResult Coordinator(int magazineId = 0)
        {
            var account = HttpContext.Session.GetString("AccountId");

            User u = _db.Users.SingleOrDefault(x => x.Id == int.Parse(account));
            if (account != null)
            {
                var facultyId = _db.Faculties.AsNoTracking().SingleOrDefault(x => x.Id == u.FacultyId);

                var contributions = _db.Contributions
                    .Include(x => x.ImgFiles)
                    .Include(x => x.Comments)
                    .Include(x => x.Magazine)
                    .Include(x => x.User)
                    .ThenInclude(x => x.Faculty)
                    .Where(x => x.User.FacultyId == x.User.Faculty.Id && x.User.Faculty.Id == facultyId.Id)
                    .OrderByDescending(x => x.Id)
                    .ToList();

                IEnumerable<Magazine> magazines = _db.Magazines.ToList();

                var stu = _db.Users
                    .Include(x => x.Role)
                    .Include(x => x.Faculty)
                    .Include(x => x.Contributions)
                    .Where(x => x.Role.Name == "Student" && x.Faculty.Id == u.FacultyId).ToList();

                var stuName = stu.Select(x => x.FullName).ToList();
                var contriCount = stu.Select(f => f.Contributions.Count()).ToList();

                var contriApproved = contributions.Where(x => x.Status == "Approved").Count();
                var contriReject = contributions.Where(x => x.Status == "Reject").Count();
                var contriProcessing = contributions.Where(x => x.Status == "Processing").Count();

                if (magazineId != 0)
                {
                   contriCount = stu.Select(f => f.Contributions.Where(x => x.Magazine.Id == magazineId).Count()).ToList();

                   contriProcessing = contributions.Where(x => x.Status == "Processing" && x.MagazineId == magazineId).Count();
                   contriReject = contributions.Where(x => x.Status == "Reject" && x.MagazineId == magazineId).Count();
                   contriApproved = contributions.Where(x => x.Status == "Approved" && x.MagazineId == magazineId).Count();
                }

               
                ViewBag.stuName = stuName;
                ViewBag.contriCount = contriCount;

                ViewBag.contriApproved = contriApproved;
                ViewBag.contriProcessing = contriProcessing;
                ViewBag.contriReject = contriReject;

                ViewBag.numberComment = contributions
                                       .Where(c => c.Comments.Any() )
                                       .Count();

                ViewBag.numberComment14Day = contributions
                                      .Where(c => !c.Comments.Any() && c.SubmissionDate.Value.AddDays(14) < DateTime.Now)
                                      .Count();

                ViewBag.numberNotComment = contributions
                                       .Where(c => !c.Comments.Any() && c.SubmissionDate.Value.AddDays(14) > DateTime.Now)
                                       .Count();

                return View(magazines);
            }
            return Redirect("/Login");
        }

        [Route("/Maketting")]
        [Authorize(Policy = "Maketting")]
        public IActionResult Maketting(int magazineId = 0)
        {
            var account = HttpContext.Session.GetString("AccountId");

            if (account != null)
            {
                IEnumerable<Contribution> contribution = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .Where(x => x.Status == "Approved")
                .ToList();

                IEnumerable<Magazine> magazines = _db.Magazines.ToList();
                

                var faculty = _db.Faculties.Include(c => c.Users).ThenInclude(x => x.Contributions).ToList();
                var lable = faculty.Select(x => x.Name).ToList();

                var facultyContributionCounts = faculty.Select(f => f.Users
                                                        .SelectMany(u => u.Contributions)
                                                        
                                                        .Count())
                                                        .ToList();

                var contriPublish = contribution.Where(x => x.Publics == true).Count();
                var contriReject = contribution.Where(x => x.Publics == false).Count();

                if (magazineId != 0)
                {
                    facultyContributionCounts = faculty.Select(f => f.Users
                                                     .SelectMany(u => u.Contributions)
                                                     .Where(x => x.MagazineId == magazineId)
                                                     .Count())
                                                     .ToList();

                    contriPublish = contribution.Where(x => x.Publics ==  true && x.MagazineId == magazineId).Count();
                    contriReject = contribution.Where(x => x.Publics == false && x.MagazineId == magazineId).Count();
                }

                ViewBag.facultyCounts = facultyContributionCounts;
                ViewBag.facultyName = lable;

                ViewBag.contriPublish = contriPublish;
                ViewBag.contriReject = contriReject;

                return View(magazines);
            }
            return Redirect("/Login");


        }

        [Route("/Student")]
        [Authorize(Policy = "Student")]
        public IActionResult Student()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {
               /* var contributions = _db.Contributions
                    .Include(x => x.ImgFiles)
                    .Include(x => x.Comments)
                    .Include(x => x.Magazine)
                    .Include(x => x.User)
                    .Where(x => x.UserId == int.Parse(account) && x.Publics == true)
                    .OrderByDescending(x => x.Id)
                    .ToList();*/

                var contri = _db.Contributions.Where(x => x.UserId == int.Parse(account)).ToList();

                var contriPro = contri.Where(c => c.Status == "Processing").Count();
                var contriReject = contri.Where(c => c.Status == "Reject").Count();
                var contriApproved = contri.Where(c => c.Status == "Approved").Count();

                ViewBag.contriPro = contriPro;
                ViewBag.contriReject = contriReject;
                ViewBag.contriApproved = contriApproved;
                ViewBag.contri = contri.Count();

                var file = _db.ImgFiles.Include(x => x.Contribution).Where(x => x.Contribution.UserId == int.Parse(account)).ToList();
                var totalFile = file.Count();
                var totalFileImg = file.Where(x => x.Stype == "Img").Count();
                var totalFiledoc = file.Where(x => x.Stype == "File").Count();

                ViewBag.totalFile = totalFile;
                ViewBag.totalFileImg = totalFileImg;
                ViewBag.totalFiledoc = totalFiledoc;

                var comment = _db.Comments.Where(x => x.UserId == int.Parse(account)).ToList();
                ViewBag.totalcomment = comment.Count();

                ViewBag.totalPublish = _db.Contributions.Include(x => x.User).Where(x => x.Publics == true && x.UserId == int.Parse(account)).Count();

                return View();
            }
            return Redirect("/Login");

        }

        [Route("/Guest/Contributions/DownloadFile/{id:}")]
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

                // Trả về tệp Zip dưới dạng phản hồi HTTP
                _notyf.Success("Download File Success");
                return File(memoryStream.ToArray(), "application/zip", "" + userName.ToString() + ".zip");

            }


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