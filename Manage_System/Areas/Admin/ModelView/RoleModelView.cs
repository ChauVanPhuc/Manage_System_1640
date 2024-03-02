using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Admin.ModelView
{
    public class RoleModelView
    {
        public int id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Full Name")]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Description")]
        [Display(Name = " Description")]
        public string Description { get; set; }
    }
}
