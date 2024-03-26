using Manage_System.models;
using Manage_System.Models;
using Manage_System.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Data;
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

        [Route("/Student/Person")]
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
            var users = await _db.Users.Include(x => x.Role).Where(x=> x.Role.Name == "Student" || x.Role.Name == "Coordinator").ToListAsync();

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
        public IActionResult Guest()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {
                var contributions = _db.Contributions
                .Include(x => x.ImgFiles)
                .Include(x => x.Comments)
                .Include(x => x.Magazine)
                .Include(x => x.User)
                .Where(x => x.Publics == true)
                .ToList();

                return View(contributions);
            }
            return Redirect("/Login");

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
        public IActionResult Coordinator()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {
                return View();
            }
            return Redirect("/Login");

        }

        [Route("/Maketting")]
        public IActionResult Maketting()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {
                return View();
            }
            return Redirect("/Login");

        }

        [Route("/Student")]
        public IActionResult Student()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {
                var contributions = _db.Contributions
                    .Include(x => x.ImgFiles)
                    .Include(x => x.Comments)
                    .Include(x => x.Magazine)
                    .Include(x => x.User)
                    .Where(x => x.UserId == int.Parse(account) && x.Publics == true)
                    .OrderByDescending(x => x.Id)
                    .ToList();

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


                return View(contributions);
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