using System.ComponentModel.DataAnnotations;

namespace Manage_System.ModelViews
{
    public class ForgotPasswordViewModel
    {
        [MaxLength(50)]
        [EmailAddress]
        [Required(ErrorMessage = "Please, Enter Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
