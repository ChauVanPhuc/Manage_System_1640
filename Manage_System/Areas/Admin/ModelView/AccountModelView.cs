using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Manage_System.Areas.Admin.ModelView
{
    public class AccountModelView
    {
        public int id {  get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please, Enter Full Name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [MaxLength(50)]
        [EmailAddress]
        [Required(ErrorMessage = "Please, Enter Email")]
        [Remote(action: "ValidateEmail", controller: "Accounts", areaName:"Admin")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(50)]
        [MinLength(5)]
        [Required(ErrorMessage = "Please, Enter Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Compare("Password", ErrorMessage = "Password Incorrect")]
        [Required(ErrorMessage = "Please, Enter Confirm Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }


        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Please, Enter Phone")]
        [Remote(action: "ValidatePhone", controller: "Accounts", areaName: "Admin", HttpMethod = "POST")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Display(Name = "Birthday")]
       
        public int? roleId { get; set; }
        public int? facultyId { get; set; }

        public IFormFile? avatar {  get; set; }
        public string? img { get; set; }
        public DateTime? createDay { get; set; }
        public bool? status { get; set; }
    }
}
