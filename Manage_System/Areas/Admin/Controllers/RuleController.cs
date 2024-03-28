using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.Areas.Admin.ModelView;
using Manage_System.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Manage_System.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class RuleController : Controller
    {
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;

        public RuleController(ManageSystem1640Context db, INotyfService notyf)
        {
            _db = db;
            _notyf = notyf;
        }

        [Route("/Admin/Rule")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Manage_System.models.Rule> rule = _db.Rules.ToList();
            return View(rule);
        }

        [Route("/Admin/Rule/Create")]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [Route("/Admin/Rule/Create")]
        public IActionResult Create(RuleModelView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    models.Rule r = new models.Rule
                    {
                        Rules = model.Rule,
                        Name = model.Name,
                    };
                    _db.Rules.Add(r);
                    _db.SaveChanges();
                    _notyf.Success("Add Rule Success");
                    return RedirectToAction("Index");
                }
                catch
                {
                    _notyf.Error("Add Rules Fail");
                    return View(model);
                }


            }
            _notyf.Error("Add Rules Fail");
            return View(model);
        }


        [Route("/Admin/Rule/Edit/{id:}")]
        public IActionResult Edit(int id)
        {
            var rule = _db.Rules.Find(id);
            if (rule == null)
            {
                _notyf.Error("Role does not exist");
                return RedirectToAction("Index");
            }

            RuleModelView model = new RuleModelView
            {
                Id = id,
                Rule = rule.Rules,
                Name = rule.Name
            };
            return View(model);
        }

        [HttpPost]
        [Route("/Admin/Rule/Edit/{id:}")]
        public IActionResult Edit(RuleModelView model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    models.Rule rules = new models.Rule
                    {
                        Id = (int)model.Id,
                        Rules = model.Rule,
                        Name = model.Name
                    };

                    _db.Rules.Update(rules);
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

        [Route("/Admin/Rule/Delete/{id:}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var rule = _db.Rules.Find(id);
                if (rule == null)
                {
                    _notyf.Error("Rules does not exist");
                    return RedirectToAction("Index");
                }
                else
                {
                    _db.Rules.Remove(rule);
                    _db.SaveChanges();

                    _notyf.Success("Delete Rules Success");

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
