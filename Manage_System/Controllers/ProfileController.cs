using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Extension;
using Manage_System.models;
using Manage_System.ModelViews;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;
        private readonly IFileService _formFile;
        public ProfileController(ILogger<HomeController> logger,
            ManageSystem1640Context db,
            INotyfService notyf,
            IFileService formFile)
        {
            _logger = logger;
            _db = db;
            _notyf = notyf;
            _formFile = formFile;
        }

        public IActionResult Index()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account == null)
            {
                return Redirect("/Login");
            }

            var user = _db.Users.Find(int.Parse(account));



            ProfileModelDisplay model = new ProfileModelDisplay
            {
                id = user.Id,
                Infor = new ChangeInforModelView
                {
                    Phone = user.Phone,
                    FullName = user.FullName,
                    img = user.Avatar,
                    Email = user.Email,
                },

                Password = new ChangePasswordModelView
                {
                    NewPassword = user.Password,
                    OldPassword = user.Password,
                    ConfirmPassword = user.Password,
                }
            };

            return View(model);
        }       

        public IActionResult ChangeInfor(ProfileModelDisplay model, string img)
        {
            try
            {
                var account = HttpContext.Session.GetString("AccountId");
                var user = _db.Users.Find(int.Parse(account));

                var phone = _db.Users.AsNoTracking().FirstOrDefault(x => x.Phone == model.Infor.Phone && x.Phone != user.Phone);
                if (phone != null)
                {
                    _notyf.Error("Fail, Phone already exist");
                    return RedirectToAction("Index");
                }

                if (model.Infor.avatar == null)
                {
                    model.Infor.img = img;
                }
                else
                {
                    img = _formFile.SaveImage(model.Infor.avatar);

                    if (model.Infor.img != null)
                    {
                        _formFile.Delete(model.Infor.img);
                    }

                    user.Avatar = img;
                }

                user.FullName = model.Infor.FullName;
                user.Phone = model.Infor.Phone;

                _db.Update(user);
                _db.SaveChanges();

                _notyf.Success("Update Information Success");
                return RedirectToAction("Index");
            }
            catch
            {

                _notyf.Error("Update Information Fail");
                return RedirectToAction("Index");
            }
            
        }

        public IActionResult ChangePassword(ProfileModelDisplay model)
        {
            try
            {
                var account = HttpContext.Session.GetString("AccountId");
                var user = _db.Users.Include(x => x.Role).AsNoTracking().SingleOrDefault(x => x.Id == int.Parse(account));

                string password = HashMD5.ToMD5(model.Password.OldPassword);
                if (user.Password != password)
                {
                    _notyf.Error("Invalid information");
                    return RedirectToAction("Index");
                }

                if (model.Password.NewPassword != model.Password.ConfirmPassword)
                {
                    _notyf.Error("No password matched");
                    return RedirectToAction("Index");
                }

                user.Password = HashMD5.ToMD5(model.Password.NewPassword);
                _db.Update(user);
                _db.SaveChanges();

                _notyf.Success("Update Password Success");
                return RedirectToAction("Index");
            }
            catch
            {
                _notyf.Success("Update Password Fail");
                return RedirectToAction("Index");
            }
            
        }
    }
}
