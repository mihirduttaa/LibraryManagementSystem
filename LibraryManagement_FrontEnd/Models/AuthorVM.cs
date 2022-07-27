namespace LibraryManagement_FrontEnd.Models
{
    public class AuthorVM
    {
        public AuthorModel Author { get; set; }
        public IEnumerable<BooksModel> Books { get; set; }
    }
}
