using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Manage_System.ModelViews
{
    public class ChangeInforModelView
    {
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Please, Enter Phone")]
        [Remote(action: "ValidatePhone", controller: "Profile", HttpMethod = "POST")]
        [MaxLength(10)]
        [MinLength(10)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public int id { get; set; }
        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Full Name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public IFormFile? avatar { get; set; }
        public string? img { get; set; }
    }
}
