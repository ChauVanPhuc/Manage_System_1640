using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Admin.ModelView;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manage_System.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class RoleController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public RoleController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }

        [Route("/Admin/Role")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Role> role = _db.Roles.ToList();
            return View(role);
        }

        [Route("/Admin/Role/Create")]
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [Route("/Admin/Role/Create")]
        public IActionResult Create(RoleModelView role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Role r = new Role
                    {
                        Name = role.Name,
                        Description = role.Description,
                    };
                    _db.Roles.Add(r);
                    _db.SaveChanges();
                    _notyf.Success("Add Role Success");
                    return RedirectToAction("Index");
                }
                catch 
                {
                    _notyf.Error("Add Role Fail");
                    return View(role);
                }
                

            }
            _notyf.Error("Add Role Fail");
            return View(role);
        }


        [Route("/Admin/Role/Edit/{id:}")]
        public IActionResult Edit(int id)
        {
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                _notyf.Error("Role does not exist");
                return RedirectToAction("Index");
            }

            RoleModelView model = new RoleModelView
            {
                id = id,
                Name = role.Name,
                Description = role.Description
            };
            return View(model);
        }

        [HttpPost]
        [Route("/Admin/Role/Edit/{id:}")]
        public IActionResult Edit(RoleModelView role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Role roles = new Role
                    {
                        Id  = role.id,
                        Name = role.Name,
                        Description = role.Description,
                    };

                    _db.Roles.Update(roles);
                    _db.SaveChanges();

                    _notyf.Success("Update Success");
                    return RedirectToAction("Index");
                }
                catch 
                {
                    _notyf.Success("Update Fail");
                    return View(role);
                }
            }
            else
            {
                _notyf.Success("Update Fail");
                return View(role);
            }
        }

        [Route("/Admin/Role/Delete/{id:}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var role = _db.Roles.Find(id);
                if (role == null)
                {
                    _notyf.Error("Role does not exist");
                    return RedirectToAction("Index");
                }
                else
                {
                    _db.Roles.Remove(role);
                    _db.SaveChanges();

                    _notyf.Success("Delete Role Success");

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
