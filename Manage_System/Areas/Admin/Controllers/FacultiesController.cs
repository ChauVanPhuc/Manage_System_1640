using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Admin.ModelView;
using Manage_System.models;
using Microsoft.AspNetCore.Mvc;

namespace Manage_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FacultiesController : Controller
    {
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public FacultiesController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }


        [Route("/Admin/Faculty")]
        public IActionResult Index()
        {
            IEnumerable<Faculty> facultyList = _db.Faculties.ToList();
            return View(facultyList);
        }

        [Route("/Admin/Faculty/Create")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [Route("/Admin/Faculty/Create")]
        public IActionResult Create(FacultiesModelView faculty)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Faculty f = new Faculty
                    {
                        Name = faculty.Name,
                        Description = faculty.Description,
                    };
                    _db.Faculties.Add(f);
                    _db.SaveChanges();
                    _notyf.Success("Add Faculty Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    _notyf.Error("Add Faculty Fail");
                    return View(faculty);
                }


            }
            _notyf.Error("Add Faculty Fail");
            return View(faculty);
        }


        [Route("/Admin/Faculty/Edit/{id:}")]
        public IActionResult Edit(int id)
        {
            var faculty = _db.Faculties.Find(id);
            if (faculty == null)
            {
                _notyf.Error("Role Faculty not exist");
                return RedirectToAction("Index");
            }

            FacultiesModelView model = new FacultiesModelView
            {
                id = id,
                Name = faculty.Name,
                Description = faculty.Description
            };
            return View(model);
        }

        [HttpPost]
        [Route("/Admin/Faculty/Edit/{id:}")]
        public IActionResult Edit(FacultiesModelView faculty)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Faculty faculties = new Faculty
                    {
                        Id = faculty.id,
                        Name = faculty.Name,
                        Description = faculty.Description,
                    };

                    _db.Faculties.Update(faculties);
                    _db.SaveChanges();

                    _notyf.Success("Update Faculty Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    _notyf.Success("Update Faculty Fail");
                    return View(faculty);
                }
            }
            else
            {
                _notyf.Success("Update Faculty Fail");
                return View(faculty);
            }
        }

        [Route("/Admin/Faculty/Delete/{id:}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var faculty = _db.Faculties.Find(id);
                if (faculty == null)
                {
                    _notyf.Error("Faculty does not exist");
                    return RedirectToAction("Index");
                }
                else
                {
                    _db.Faculties.Remove(faculty);
                    _db.SaveChanges();

                    _notyf.Success("Delete Faculty Success");

                    return RedirectToAction("Index");
                }
            }
            catch
            {

                _notyf.Error("Delete Faculty Fail");
                return RedirectToAction("Index");
            }


        }
    }
}
