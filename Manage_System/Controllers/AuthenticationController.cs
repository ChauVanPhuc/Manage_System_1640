using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Extension;
using Manage_System.models;
using Manage_System.ModelViews;
using Manage_System.Service;
using Manage_System.Views.Profile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Manage_System.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;
        private readonly IFileService _fileService;
        private readonly IEmailService _email;
        private readonly IUserService _userService;
        public AuthenticationController(ManageSystem1640Context db, IEmailService email, IUserService userService,
            INotyfService notyf, IFileService fileService)
        {
            _db = db;
            _notyf = notyf;
            _fileService = fileService;
            _userService = userService;
            _email = email;
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

/*        [Route("/Register")]
        public IActionResult Register()
        {
            ViewData["facultyId"] = new SelectList(_db.Faculties, "Id", "Name").ToList();

            return View();
        }*/


        [HttpPost]
        [Route("/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var account = await _db.Users.Include(x => x.Role).AsNoTracking().SingleOrDefaultAsync(x => x.Email == model.Email);
                    if (account == null)
                    {
                        _notyf.Error("Account is not registered ");
                        return View();
                    }

                    string password = HashMD5.ToMD5(model.Password);
                    if (account.Password != password)
                    {
                        _notyf.Error("Invalid information");
                        return View();
                    }

                    ////check account disable ?
                    if (account.Status == false)
                    {
                        _notyf.Error("Account Block");
                        return View();
                    }


                    //Save Session 
                    HttpContext.Session.SetString("AccountId", account.Id.ToString());
                    var accountId = HttpContext.Session.GetString("AccountId");

                    //Identity
                    var claim = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, account.FullName),
                        new Claim("AccountId", account.Id.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                        new Claim(account.Role.Name,account.Role.Name),
                    };

                    Information.avatar = account.Avatar;

                    ClaimsIdentity claims = new ClaimsIdentity(claim, "AccountId"
                        /*Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme*/
                        );
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claims);
                    
                    await HttpContext.SignInAsync(/*CookieAuthenticationDefaults.AuthenticationScheme*/ "AccountId", claimsPrincipal);

                    

                    LastLogin lastlogin = await _db.LastLogins.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == account.Id);

                    
                    
                    if (lastlogin == null)
                    {
                        _notyf.Success("Login Success");

                        LastLogin lastLogin = new LastLogin
                        {
                            UserId = account.Id,
                            History = DateTime.Now
                        };
                        _db.LastLogins.Add(lastLogin);
                        _db.SaveChanges();

                        TempData["TextLogin"] = "Welcome";
                    }
                    else
                    {
                        _notyf.Success("Login Success");
                        

                        TempData["TextLogin"] = "Wellcom Back";
                        TempData["TextLastLogin"] = "Last Login: "+lastlogin.History+" ";
                        lastlogin.History = DateTime.Now;

                        _db.Update(lastlogin);
                        _db.SaveChanges();
                    }


                    TempData["ShowLoginSuccessModal"] = "true";
                    return CheckLogin(account);

                }
            }
            catch
            {
                _notyf.Success("Login Fail");
                return View(model);
            }
            _notyf.Success("Login Fail");
            return View(model);
        }


/*        [HttpPost]
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
        }*/

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

        [HttpGet]
        [Route("/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("/ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userService.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _notyf.Error("Invalid email, please try again");
                    return View();
                }

                var token = await _userService.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Authentication", new { token, email = user.Email }, Request.Scheme);

                await _email.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: {callbackUrl}. Time one miius");

                _notyf.Success("Check your email to reset password");
                return Redirect("/login");
            }
            catch 
            {
                // Log the exception details here
                _notyf.Error("An error occurred while processing your request.");
                return View(model);
            }
        }


        [HttpGet]
        [Route("/ResetPassword")]
        public IActionResult ResetPassword(string token, string email)
        {
           
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }



        [HttpPost]
        [Route("/ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);
            if (user == null || user.PasswordResetToken != model.Token || user.TokenExpiration < DateTime.UtcNow)
            {
                
                return View(model);
            }

            if (user.Password == HashMD5.ToMD5(model.NewPassword))
            {
                _notyf.Error("This is your old password,please try again");
                return View(model);
            }
            
            user.Password = HashMD5.ToMD5(model.NewPassword); 
            user.PasswordResetToken = null; 
            user.TokenExpiration = null; 
            await _db.SaveChangesAsync();

            _notyf.Success("Reset Password Success");
            return Redirect("/Login");
        }
    }
}
