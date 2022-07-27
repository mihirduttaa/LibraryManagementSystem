using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryManagement_FrontEnd.Controllers
{
    public class AccountController : Controller
    {
        private IConfiguration _Configure;
        private string apiAccountsUrl;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,IConfiguration configuration,ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _Configure = configuration;
            _db = db;
            apiAccountsUrl = _Configure.GetValue<string>("WebAPIAccountsUrl");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UpdateUser(string Id)
        {
            ApplicationUser user = new ApplicationUser();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri($"{apiAccountsUrl}/GetUserById?Id={Id}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiAccountsUrl}/GetUserById?Id={Id}");
                if (Res.IsSuccessStatusCode)
                {
                    var UserResponse = Res.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<ApplicationUser>(UserResponse);
                    user.Email = user.UserName;
                }
                
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(ApplicationUser obj)
        {
            obj.UserName = obj.Email;
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                string endpoint = apiAccountsUrl + "/UpdateUser";
                using (var Response = await client.PutAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        TempData["success"] = "User Updated SuccessFully";
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        TempData["error"] = "User Updation Failed!";
                        return View();
                    }
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                string endpoint = apiAccountsUrl + $"/DeleteUser?id={id}";
                using (var Response = await client.DeleteAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Json(new { success = true, message = "Delete Successfull" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error while deleting" });

                    }
                }
            }
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            ApplicationUser user = new ApplicationUser();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser user)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string endpoint = apiAccountsUrl + "/register";
                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        TempData["success"] = "User Registerd Successfully!";
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError(string.Empty, "Password and Confirm Password should be same!");
                        return View();
                    }
                }
            }
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            LoginModel user = new LoginModel();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user,string IsAdmin)
        {
                var userFromDB = _db.Users.SingleOrDefault(u=>u.UserName==user.Email);
            if (IsAdmin == "true"){
                if (userFromDB == null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                    return View();
                }
                if (!userFromDB.IsAdmin)
                {
                    ModelState.Clear();
                    ModelState.AddModelError(string.Empty, "Entered Email doesn't belongs to Admin!");
                    return View();
                }
            }
            else
            {
                if (userFromDB == null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                    return View();
                }
                if (userFromDB.IsAdmin)
                {
                    ModelState.Clear();
                    ModelState.AddModelError(string.Empty, "Entered Email belongs to Admin!");
                    return View();
                }
            }
            
            var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);
            if (result.Succeeded)
            {
                if (user.RememberMe)
                {
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Append("uname", user.Email, cookieOptions);
                }
                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                return View();
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
