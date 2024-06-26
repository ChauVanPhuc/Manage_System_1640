﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Coordinator.ModelView;
using Manage_System.models;

using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace Manage_System.Areas.Coordinator.Controllers
{
    [Area("Coordinator")]
    [Authorize(Policy = "Coordinator")]
    public class ContributionController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly IFileService _formFile;
        private readonly INotyfService _notyf;
        private readonly IEmailService _email;
        private readonly IWebHostEnvironment _env;

        public ContributionController(ManageSystem1640Context db, IWebHostEnvironment env, IFileService formFile, INotyfService notyf, IEmailService email)
        {
            _db = db;
            _formFile = formFile;
            _notyf = notyf;
            _email = email;
            _env = env;
        }

        [Route("Coordinator/Contributions")]
        public async Task<IActionResult> Index()
        {
            var account = HttpContext.Session.GetString("AccountId");
            var user = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));
            var facultyId = _db.Faculties.AsNoTracking().SingleOrDefault(x => x.Id == user.FacultyId);

            var contributions = await _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .ThenInclude(x => x.Faculty)
                .Where(x => x.User.FacultyId == x.User.Faculty.Id && x.User.Faculty.Id == facultyId.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            if (contributions == null)
            {
                return NotFound();
            }
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
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault(b => b.Id == id);

                List<Comment> comments = _db.Comments.Include(x => x.User).Where(x => x.ContributionId == id).OrderByDescending(x => x.Id).ToList();

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
                    if (contri.Status == "Processing" || contri.Status == "Reject")
                    {
                        contri.Status = "Approved";

                        var contributions = _db.Contributions.AsNoTracking().SingleOrDefault(x => x.Id == id);
                        var account = HttpContext.Session.GetString("AccountId");
                        var user = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));
                        var student = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == contributions.UserId);

                        _email.SendEmailAsync(student.Email, "Contribution has been Approved", " Your post "+ contributions.Title+ " has been approved");

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

        [Route("/Coordinator/Contributions/Reject/{id:}")]
        public IActionResult Reject(int id)
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
                    if (contri.Status == "Processing" || contri.Status == "Approved")
                    {
                        contri.Status = "Reject";

                        if (contri.Publics == true)
                        {
                            contri.Publics = false;
                        }

                        var contributions = _db.Contributions.AsNoTracking().SingleOrDefault(x => x.Id == id);
                        var account = HttpContext.Session.GetString("AccountId");
                        var user = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));
                        var student = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == contributions.UserId);

                        _email.SendEmailAsync(student.Email, "Contribution has been Reject", " Your post " + contributions.Title + " has been Reject");

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

        [Route("/Coordinator/Contributions/Person")]
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

        [Route("/Coordinator/Contributions/Chat/{id:}")]
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

        [Route("/Coordinator/Contributions/DownloadFile/{id:}")]
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
                
                return File(memoryStream.ToArray(), "application/zip", "" + userName.ToString() + ".zip");

            }


        }
    }
}
