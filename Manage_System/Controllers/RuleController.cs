using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.ModelViews;
using Manage_System.Service;
using Microsoft.AspNetCore.Mvc;

namespace Manage_System.Controllers
{
    public class RuleController : Controller
    {
        private readonly ManageSystem1640Context _db;
        private readonly INotyfService _notyf;
        private readonly IEmailService _emailService;
        public RuleController(ManageSystem1640Context db, INotyfService notyf, IEmailService emailService)
        {
            _db = db;
            _notyf = notyf;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            IEnumerable<Rule> rules = _db.Rules.ToList();
            IEnumerable<Security> security = _db.Securities.ToList();

            RulesModelView model = new RulesModelView
            {
                Rules = rules,
                Security = security,
            };

            return View(model);
        }
    }
}
