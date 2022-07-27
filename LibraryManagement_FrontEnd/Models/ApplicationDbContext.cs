using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement_FrontEnd.Models
{
    public class ApplicationDbContext:IdentityDbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<BooksModel> Books { get; set; }
        public DbSet<AuthorModel> Authors { get; set; }
        public DbSet<PublisherModel> Publishers { get; set; }
        public DbSet<LendBookRequests> LendRequests { get; set; }
    }
}
