using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement_FrontEnd.Models
{
    public class BooksModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Publisher { get; set; }
        
        [ValidateNever]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }


        public int AvailableCount { get; set; } = 0;



    }
}
