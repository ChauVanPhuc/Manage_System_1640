using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Admin.ModelView;
using Manage_System.Controllers;
using Manage_System.Extension;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Manage_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class AccountsController : Controller
    {
        private readonly ManageSystem1640Context _db;
        private readonly IFileService _formFile;
        private readonly INotyfService _notyf;

        public AccountsController(ManageSystem1640Context db, IFileService formFile, INotyfService notyf)
        {
            _db = db;
            _formFile = formFile;
            _notyf = notyf;
        }


        [Route("/Admin/Accounts/ValidatePhone")]
        [HttpPost]
        public async Task<IActionResult> ValidatePhone(string phone)
        {
            try
            {
                var account = _db.Users.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == phone.ToLower());
                if (account == null)
                {
                    
                    return Json(true);
                }
                else
                {   
                    return Json($"Phone: {phone} already exist");
                }
                
            }
            catch
            {

                return Json(true);
            }
        }

        [Route("/Admin/Accounts/ValidateEmail")]
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string email)
        {
            try
            {
                var account = _db.Users.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == email.ToLower());
                if (account != null)
                {
                    return Json(data: "Email: " + email + " already exist");

                }
                return Json(data: true);
            }
            catch
            {

                return Json(data: true);
            }
        }


        [Route("/Admin/Accounts")]
        public async Task<IActionResult> Index()
        {
            var account = await _db.Users
                .Include(x => x.Role)
                .Include(x => x.Faculty)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            return View(account);
        }


        private void RoleFac()
        {
            ViewData["roleId"] = new SelectList(_db.Roles, "Id", "Name").ToList();
            ViewData["facultyId"] = new SelectList(_db.Faculties, "Id", "Name").ToList();
        }

        [Route("/Admin/Accounts/Create")]
        public IActionResult Create()
        {
            RoleFac();
            return View();
        }



        [HttpPost]
        [Route("/Admin/Accounts/Create")]
        public IActionResult Create(AccountModelView model)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    string img = "";
                    if (model.avatar != null)
                    {
                        img = _formFile.SaveImage(model.avatar);

                    }
                    
                    User account = new User
                    {
                        Email = model.Email,
                        Password = HashMD5.ToMD5(model.Password),
                        Phone = model.Phone,
                        RoleId = model.roleId,
                        Status = true,
                        CreateDay = DateTime.Now,
                        FullName = model.FullName,
                        FacultyId = model.facultyId,
                        Avatar = img
                    };

                    try
                    {
                        _db.Add(account);
                        _db.SaveChangesAsync();

                        _notyf.Success("Register Success");
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        RoleFac();
                        _notyf.Error("Register Fail");
                        return View(model);
                    }
                }
                else
                {
                    RoleFac();
                    _notyf.Error("Register Fail");
                    return View(model);
                }
            }
            catch
            {
                RoleFac();
                _notyf.Error("Register Fail");
                return View(model);
            }
        }


        [Route("/Admin/Accounts/Edit/{id:}")]
        public IActionResult Edit(int id)
        {
            try
            {
                var account = _db.Users.Find(id);
                if (account == null)
                {
                    _notyf.Error("Account does not exist");
                    return RedirectToAction("Index");
                }

                AccountEditModel model = new AccountEditModel
                {
                    id = id,
                    Email = account.Email,
                    Password = HashMD5.ToMD5(account.Password),
                    ConfirmPassword = HashMD5.ToMD5(account.Password),
                    FullName = account.FullName,
                    Phone = account.Phone,
                    roleId = account.RoleId,
                    facultyId = account.FacultyId,
                    createDay = account.CreateDay,
                    status = account.Status,
                    img = account.Avatar
                };

                RoleFac();
                return View(model);
            }
            catch
            {
                _notyf.Error("Edit Fail");
                return RedirectToAction("Index");
            }
            
        }


        [HttpPost]
        [Route("/Admin/Accounts/Edit/{id:}")]
        public IActionResult Edit(AccountEditModel model, string img)
        {
            try
            {
                    if (model.avatar == null)
                    {
                        model.img = img;
                    }
                    else
                    {
                        img = _formFile.SaveImage(model.avatar);
                        
                        if(model.img != null)
                        {
                            _formFile.Delete(model.img);
                        }
                    }

                    User account = new User
                    {
                        Id = model.id,
                        Password = model.Password,
                        Email = model.Email,
                        FullName = model.FullName,
                        Phone = model.Phone,
                        RoleId = model.roleId,
                        FacultyId = model.facultyId,
                        CreateDay = model.createDay,
                        Status = model.status,
                        Avatar = img
                    };

                    _db.Update(account);
                    _db.SaveChanges();

                    _notyf.Success("Update Account Success");
                    return RedirectToAction("Index");

            }
            catch 
            {
                _notyf.Error("Edit Fail");
                return RedirectToAction("Index");
            }
            
        }


        [Route("/Admin/Account/Delete/{id:}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var acc = _db.Users.Find(id);
                if (acc == null)
                {
                    _notyf.Error("Account does not exist");
                    return RedirectToAction("Index");
                }
                else
                {
                    if (acc.Status == false)
                    {
                        acc.Status = true;
                        _notyf.Success("Account Disable Success");
                    }
                    else
                    {
                        acc.Status = false;
                        _notyf.Success("Account Active Success");
                    }                 
                    _db.Update(acc);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {

                _notyf.Error("Delete Fail");
                return RedirectToAction("Index");
            }


        }
    }
}
