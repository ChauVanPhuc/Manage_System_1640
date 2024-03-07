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
        [Required(ErrorMessage = "Please, Enter Close Day")]
        [Display(Name = "Closure Day")]
        public string? ClosureDay { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Final Close Day")]
        [Display(Name = "Final Closure Day")]
        public string? FinalClosureDay { get; set; }
    }
}
