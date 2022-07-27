using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement_FrontEnd.Models
{
    public class ApplicationUser:IdentityUser
    {
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password should be same")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
