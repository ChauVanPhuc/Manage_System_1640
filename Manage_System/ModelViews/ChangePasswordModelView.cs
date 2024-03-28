using System.ComponentModel.DataAnnotations;

namespace Manage_System.ModelViews
{
    public class ChangePasswordModelView
    {
        [MaxLength(50)]
        [MinLength(5)]
        [Required(ErrorMessage = "Please, Enter Old Password")]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter New Password")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Password Incorrect")]
        [Required(ErrorMessage = "Please, Enter Confirm Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
