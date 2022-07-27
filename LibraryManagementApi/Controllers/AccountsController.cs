using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        private IPasswordHasher<ApplicationUser> passwordHasher;
        public ApplicationDbContext _db { get; set; }

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,ApplicationDbContext db, IPasswordHasher<ApplicationUser> passwordHash)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            passwordHasher = passwordHash;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                return Ok(_db.Users);
            }
            catch
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        public IActionResult GetUserById(string id)
        {
            try
            {
                return Ok(_db.Users.Single(u => u.Id == id));
            }
            catch
            {
                return NotFound();
            }
        }
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(ApplicationUser Model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(Model.Id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(Model.Email))
                {
                    user.Email = Model.Email;
                    user.UserName = Model.Email;
                }
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(Model.Name))
                {
                    user.Name = Model.Name;
                }
                if (!string.IsNullOrEmpty(Model.Password))
                    user.PasswordHash = passwordHasher.HashPassword(user, Model.Password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(Model.Email) && !string.IsNullOrEmpty(Model.Password))
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return Ok(user);
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");
            try
            {
                _db.Users.Remove(_db.Users.Single(u=>u.Id==id));
                _db.SaveChanges();
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser Model)
        {
            ApplicationUser user = new ApplicationUser() { UserName = Model.Email, Email = Model.Email, Name = Model.Name,IsAdmin=Model.IsAdmin};
            var result = await _userManager.CreateAsync(user, Model.Password);

            if (result.Succeeded)
            {
                return Ok(user);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Errors", error.Description);

                }
                return BadRequest(ModelState);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel Model)
        {
            var result = await _signInManager.PasswordSignInAsync(Model.Email, Model.Password, Model.RememberMe, false);
            if (result.Succeeded)
            {
                return Ok("Login Succesful");
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Login Successful");
        }

    }
}
