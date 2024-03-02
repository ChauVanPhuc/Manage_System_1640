using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Admin.ModelView
{
    public class FacultiesModelView
    {
        public int id { get; set; }


        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Description")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
