using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Coordinator.ModelView;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Mvc;

namespace Manage_System.Areas.Coordinator.Controllers
{
    public class CommentController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public CommentController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }

        [HttpPost]
        [Route("Coordinator/SubmitComment/{id:}")]
        public IActionResult SubmitComment(int id, ContributionsModelView model)
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
