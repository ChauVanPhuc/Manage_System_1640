using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Admin.ModelView;
using Manage_System.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manage_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class SecurityController : Controller
    {
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public SecurityController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }

        [Route("/Admin/Security")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Security> security = _db.Securities.ToList();
            return View(security);
        }

        [Route("/Admin/Security/Create")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [Route("/Admin/Security/Create")]
        public IActionResult Create(SecurityModelView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Security r = new models.Security
                    {
                        Security1 = model.Security,
                        Name = model.Name,
                    };
                    _db.Securities.Add(r);
                    _db.SaveChanges();
                    _notyf.Success("Add Security Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    _notyf.Error("Add Security Fail");
                    return View(model);
                }


            }
            _notyf.Error("Add Security Fail");
            return View(model);
        }


        [Route("/Admin/Security/Edit/{id:}")]
        public IActionResult Edit(int id)
        {
            var s = _db.Securities.Find(id);
            if (s == null)
            {
                _notyf.Error("Security does not exist");
                return RedirectToAction("Index");
            }

            SecurityModelView model = new SecurityModelView
            {
                Id = id,
                Security = s.Security1,
                Name = s.Name
            };
            return View(model);
        }

        [HttpPost]
        [Route("/Admin/Security/Edit/{id:}")]
        public IActionResult Edit(SecurityModelView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    models.Security security = new models.Security
                    {
                        Id = (int)model.Id,
                        Security1 = model.Security,
                        Name = model.Name
                    };

                    _db.Securities.Update(security);
                    _db.SaveChanges();

                    _notyf.Success("Update Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    _notyf.Success("Update Fail");
                    return View(model);
                }
            }
            else
            {
                _notyf.Success("Update Fail");
                return View(model);
            }
        }

        [Route("/Admin/Security/Delete/{id:}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var security = _db.Securities.Find(id);
                if (security == null)
                {
                    _notyf.Error("security does not exist");
                    return RedirectToAction("Index");
                }
                else
                {
                    _db.Securities.Remove(security);
                    _db.SaveChanges();

                    _notyf.Success("Delete security Success");

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
