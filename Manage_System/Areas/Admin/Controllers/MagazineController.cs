using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Admin.ModelView;
using Manage_System.models;
using Manage_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace Manage_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MagazineController : Controller
    {

        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public MagazineController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }

        [Route("/Admin/Magazine")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Magazine> magazines = _db.Magazines.ToList();

            return View(magazines);
        }

        [Route("/Admin/Magazine/Create")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [Route("/Admin/Magazine/Create")]
        public IActionResult Create(MagazineModelView magazine)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Magazine r = new Magazine
                    {
                        Description = magazine.Description,
                        StartYear = DateTime.Parse(magazine.StartYear),
                        CloseYear = DateTime.Parse(magazine.CloseYear)
                    };

                    _db.Magazines.Add(r);
                    _db.SaveChanges();
                    _notyf.Success("Add Magazine Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    _notyf.Error("Add Magazine Fail");
                    return View(magazine);
                }


            }
            _notyf.Error("Add Magazine Fail");
            return View(magazine);
        }


        [Route("/Admin/Magazine/Edit/{id:}")]
        public IActionResult Edit(int id)
        {
            var magazine = _db.Magazines.Find(id);
            if (magazine == null)
            {
                _notyf.Error("Magazine does not exist");
                return RedirectToAction("Index");
            }

            MagazineModelView model = new MagazineModelView
            {
                id = id,
                Description = magazine.Description,
                CloseYear = magazine.CloseYear.Value.ToString("yyyy-MM-dd"),
                StartYear = magazine.StartYear.Value.ToString("yyyy-MM-dd")

            };

            return View(model);
        }

        [HttpPost]
        [Route("/Admin/Magazine/Edit/{id:}")]
        public IActionResult Edit(MagazineModelView magazine)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Magazine m = new Magazine
                    {
                        Id = magazine.id,
                        Description = magazine.Description,
                        CloseYear = DateTime.Parse(magazine.CloseYear) ,
                        StartYear = DateTime.Parse(magazine.StartYear)
                    };

                    _db.Magazines.Update(m);
                    _db.SaveChanges();

                    _notyf.Success("Update Magazine Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    _notyf.Success("Update Magazine Fail");
                    return View(magazine);
                }
            }
            else
            {
                _notyf.Success("Update Magazine Fail");
                return View(magazine);
            }
        }

        [Route("/Admin/Magazine/Delete/{id:}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var magazine = _db.Magazines.Find(id);
                if (magazine == null)
                {
                    _notyf.Error("Magazine does not exist");
                    return RedirectToAction("Index");
                }
                else
                {
                    _db.Magazines.Remove(magazine);
                    _db.SaveChanges();

                    _notyf.Success("Delete Magazine Success");

                    return RedirectToAction("Index");
                }
            }
            catch
            {

                _notyf.Error("Delete Magazine Fail");
                return RedirectToAction("Index");
            }


        }

    }
}
