using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Extension;
using Manage_System.models;
using Manage_System.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Manage_System.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public AuthenticationController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }


        [Route("/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = _db.Users.AsNoTracking().SingleOrDefault(x => x.Email == model.Email);
                    if (account == null)
                    {
                        _notyf.Error("Account is not registered ");
                        return Redirect("/Login");
                    }

                    string password = HashMD5.ToMD5(model.Password);
                    if (account.Password != password)
                    {
                        _notyf.Error("Invalid information");
                        return View(model);
                    }

                    ////check account disable ?
                    //if (account.Status == false) return RedirectToAction("Mess", "Account");

                    //Save Session 
                    HttpContext.Session.SetString("AccountId", account.Id.ToString());
                    var accountId = HttpContext.Session.GetString("AccountId");

                    //Identity
                    var claim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.FullName),
                        new Claim("AccountId", account.Id.ToString())
                    };
                    ClaimsIdentity claims = new ClaimsIdentity(claim, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claims);

                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyf.Success("Login Success");
                    return Redirect("/Home");
                }
            }
            catch
            {
                _notyf.Success("Login Fail");
                return Redirect("/Login");
            }
            _notyf.Success("Login Fail");
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccountId");
            return Redirect("/Home");
        }
    }
}
