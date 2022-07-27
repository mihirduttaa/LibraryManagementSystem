using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("Policy1")]
    public class BooksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public BooksApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]

        public IActionResult GetAllBooks()
        {
            try
            {
                return Ok(_db.Books);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]

        public IActionResult GetBookById(int id)
        {
            try
            {
                return Ok(_db.Books.Single(u=>u.Id==id));
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddBook(BooksModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {
               
                _db.Books.Add(Model);
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBook(BooksModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            try
            {
                _db.Books.Update(Model);
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            try
            {
                _db.Books.Remove(_db.Books.Find(id));
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
