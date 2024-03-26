using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Admin.ModelView
{
    public class RuleModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please, Enter Rule")]
        [Display(Name = "Rule")]
        public string Rule { get; set; }
        [Required(ErrorMessage = "Please, Enter Rule Name")]
        [Display(Name = "Rule Name")]
        public string Name { get; set; }
    }
}
