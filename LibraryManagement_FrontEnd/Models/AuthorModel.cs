using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement_FrontEnd.Models
{
    public class AuthorModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Country { get; set; }
        public string Description { get; set; }
        
        [ValidateNever]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
    }
}
