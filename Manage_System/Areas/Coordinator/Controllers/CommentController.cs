using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Coordinator.ModelView;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Areas.Coordinator.Controllers
{
    [Authorize(Policy = "Coordinator")]
    [Area("Coordinator")]
    public class CommentController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public CommentController(ManageSystem1640Context db, INotyfService notyf, IEmailService emailService)
        {
            _db = db;
            _notyf = notyf;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("Coordinator/SubmitComment/{id:}")]
        public async Task<IActionResult> SubmitComment(int id, ContributionsModelView model)
        {
            var contribution = _db.Contributions.Find(id);
            if (contribution == null)
            {
                _notyf.Error("contribution does not exist");
                return Redirect("/Coordinator/Contributions/");
            }

            
                try
                {
                    var account = HttpContext.Session.GetString("AccountId");
                    Comment comment = new Comment 
                    {
                       CommentText = model.Comment,
                       UserId = int.Parse(account),
                       CommentDate = DateTime.Now,
                       ContributionId = model.Id
                    };

                    _db.Comments.Add(comment);
                    _db.SaveChanges();

                var student = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == contribution.UserId);
                var coordinator = _db.Users.AsNoTracking().SingleOrDefault(x => x.Id == comment.UserId);
                if (student != null)
                {
                    await _emailService.SendEmailAsync(student.Email, "Contribution Comment", coordinator.FullName + " has made a new Comment");

                }
                    _notyf.Success("Comment Success");
                    return Redirect("/Coordinator/Contributions/Detail/" + model.Id + " ");
                }
                catch (Exception)
                {

                    _notyf.Success("Comment Fail");
                    return Redirect("/Coordinator/Contributions/Detail/"+model.Id + " ");
                }

        }
    }
}
