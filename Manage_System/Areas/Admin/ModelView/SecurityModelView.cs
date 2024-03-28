using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Admin.ModelView
{
    public class SecurityModelView
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Please, Enter Security")]
        [Display(Name = "Security")]
        public string Security { get; set; }
        [Required(ErrorMessage = "Please, Enter Security Name")]
        [Display(Name = "Security Name")]
        public string Name { get; set; }
    }
}
