using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublishersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PublishersApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]

        public IActionResult GetAllPublishers()
        {
            try
            {
                return Ok(_db.Publishers);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        public IActionResult GetPublisherById(int id)
        {
            try
            {
                return Ok(_db.Publishers.Single(u => u.Id == id));
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPublisher(PublisherModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {

                _db.Publishers.Add(Model);
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePublisher(PublisherModel Model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            try
            {
                _db.Publishers.Update(Model);
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            try
            {
                _db.Publishers.Remove(_db.Publishers.Find(id));
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
