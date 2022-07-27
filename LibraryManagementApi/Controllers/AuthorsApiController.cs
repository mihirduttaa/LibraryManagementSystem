using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("Policy1")]
    public class AuthorsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public AuthorsApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]

        public IActionResult GetAllAuthors()
        {
            try
            {
                return Ok(_db.Authors);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        public IActionResult GetAuthorById(int id)
        {
            try
            {
                return Ok(_db.Authors.Single(u=>u.Id==id));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAuthor(AuthorModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {

                _db.Authors.Add(Model);
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAuthor(AuthorModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            try
            {
                _db.Authors.Update(Model);
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            try
            {
                _db.Authors.Remove(_db.Authors.Find(id));
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
