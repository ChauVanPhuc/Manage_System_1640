using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Extension;
using Manage_System.models;
using Manage_System.ModelViews;
using Manage_System.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Manage_System.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;
        private readonly IFileService _fileService;

        public AuthenticationController(ManageSystem1640Context db, INotyfService notyf, IFileService fileService)
        {
            _db = db;
            _notyf = notyf;
            _fileService = fileService;
        }

        [Route("/Login")]
        public IActionResult Login()
        {
            var account = HttpContext.Session.GetString("AccountId");
            if (account != null)
            {
                return Redirect("/");
            }
            return View();
        }

        [Route("/Register")]
        public IActionResult Register()
        {
            ViewData["facultyId"] = new SelectList(_db.Faculties, "Id", "Name").ToList();

            return View();
        }


        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = _db.Users.Include(x => x.Role).AsNoTracking().SingleOrDefault(x => x.Email == model.Email);
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
                    ClaimsIdentity claims = new ClaimsIdentity(claim, 
                        Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme
                        );
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claims);
                    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme ,claimsPrincipal);

                    _notyf.Success("Login Success");
                    return CheckLogin(account);

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


        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> Register(RegisterModelView model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string img = null;
                    if (model.avatar != null)
                    {
                        img = _fileService.SaveImage(model.avatar);

                    }


                    var role = _db.Roles.FirstOrDefault(x => x.Name == "Student");
                    int roles = 0;

                    if (role != null)
                    {
                        roles = role.Id;
                    }
                    else
                    {
                        Role r = new Role
                        {
                            Name = "Guest",
                            Description = "Guest"
                        };

                        _db.Roles.Add(r);
                        _db.SaveChanges();

                        roles = r.Id;
                    }

                    User account = new User
                    {
                        Email = model.Email,
                        Password = HashMD5.ToMD5(model.Password),
                        Phone = model.Phone,
                        RoleId = roles,
                        Status = true,
                        CreateDay = DateTime.Now,
                        FullName = model.FullName,
                        Avatar = img,
                        FacultyId = model.FacultyId,
                       
                    };

                    try
                    {
                        _db.Add(account);
                        await _db.SaveChangesAsync();

                        //Save session 
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
                        _notyf.Success("Register Success");
                        return Redirect("/");
                    }
                    catch
                    {
                        return Redirect("/Register");
                    }
                }
                else
                {
                    _notyf.Error("Register Fail");
                    return View(model);
                }
            }
            catch
            {
                _notyf.Error("Register Fail");
                return View(model);
            }
        }

        [HttpPost]
        [Route("/Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return Redirect("/Login");
        }

        public IActionResult CheckLogin(User accountId)
        {
            string role = CheckRole.CheckRoleLogin(accountId);
            if (role == "Guest")
            {
                return Redirect("/");
            }else if (role == "Student")
            {
                return Redirect("/Student");
            }else if (role == "Coordinator")
            {
                return Redirect("/Coordinator");
            }else if (role == "Maketting")
            {
                return Redirect("/Maketting");
            }
            else
            {
                return Redirect("/Admin");
            }
        }
    }
}
