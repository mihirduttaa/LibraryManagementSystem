using System.ComponentModel.DataAnnotations;

namespace LibraryManagement_FrontEnd.Models
{
    public class LendBookRequests
    {
        public int Id { get; set; }
        [Required]
        public string BookName { get; set; }

        [Required]
        public string CustomerUserName { get; set; }
        public string Status { get; set; } = "Pending";

        public DateTime LendDate { get; set; } = DateTime.Now.Date;

        public DateTime ExpectedReturnDate { get; set; } = DateTime.Now.AddDays(20).Date;

        public DateTime? ActualReturnDate { get; set; }
    }
}
