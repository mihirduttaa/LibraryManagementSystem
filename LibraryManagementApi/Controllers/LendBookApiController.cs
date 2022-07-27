using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("Policy1")]
    public class LendBookApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LendBookApiController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult GetAllLendBookRequests()
        {
            try
            {
                return Ok(_db.LendRequests);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLendBookRequestById(int id)
        {
            try
            {
                LendBookRequests request = _db.LendRequests.Single(u => u.Id == id);
                
                return Ok(request);
            }
            catch
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLendBookRequestsByUserName(string username)
        {
            try
            {
                List<LendBookRequests> LendBookRequests = _db.LendRequests.ToList();
                
                LendBookRequests = LendBookRequests.Where(u => u.CustomerUserName == username).ToList();
                return Ok(LendBookRequests);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteLendBookRequestById(int id)
        {
            try
            {
                LendBookRequests request = _db.LendRequests.Single(u => u.Id == id);
                _db.LendRequests.Remove(request);
                _db.SaveChanges();
                return Ok(request);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
