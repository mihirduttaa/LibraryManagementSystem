namespace LibraryManagement_FrontEnd.Models
{
    public class PublisherVM
    {
        public PublisherModel Publisher { get; set; }
        public IEnumerable<BooksModel> Books { get; set; }
    }
}
