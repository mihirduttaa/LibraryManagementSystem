using System.ComponentModel.DataAnnotations;

namespace LibraryManagement_FrontEnd.Models
{
    public class PublisherModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Country { get; set; }
        public string Description { get; set; }
    }

}
