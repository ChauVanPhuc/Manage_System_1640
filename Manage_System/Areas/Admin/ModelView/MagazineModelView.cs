using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Admin.ModelView
{
    public class MagazineModelView
    {
        public int id { get; set; }
        

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Description")]
        [Display(Name = " Description")]
        public string Description { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Close Years")]
        [Display(Name = "Close Years")]
        public string? StartYear { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Close Years")]
        [Display(Name = "Close Years")]
        public string? CloseYear { get; set; }
    }
}
