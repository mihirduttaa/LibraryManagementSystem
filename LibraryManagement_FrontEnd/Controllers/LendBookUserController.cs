using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LibraryManagement_FrontEnd.Controllers
{
    public class LendBookUserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string apiLendBookRequestsUrl;
        private IConfiguration _Configure;

        public LendBookUserController(IConfiguration configuration,ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _Configure = configuration;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            apiLendBookRequestsUrl = _Configure.GetValue<string>("WebAPILendBookRequestsUrl");
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLendRequestByUser()
        {
            List<LendBookRequests> request = new List<LendBookRequests>();
            using (var client = new HttpClient())
            {
                var user = await _userManager.GetUserAsync(User);
                client.BaseAddress = new Uri($"{apiLendBookRequestsUrl}/GetAllLendBookRequestsByUserName?username={user.UserName}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiLendBookRequestsUrl}/GetAllLendBookRequestsByUserName?username={user.UserName}");
                if (Res.IsSuccessStatusCode)
                {
                    var LendBookRequestsResponse = Res.Content.ReadAsStringAsync().Result;
                    request = JsonConvert.DeserializeObject<List<LendBookRequests>>(LendBookRequestsResponse);
                    return Ok(request);
                }
                return NotFound();
            }
        }

        public async Task<IActionResult> ViewBill()
        {
            List<LendBookRequests> request = new List<LendBookRequests>();
            using (var client = new HttpClient())
            {
                var user = await _userManager.GetUserAsync(User);
                client.BaseAddress = new Uri($"{apiLendBookRequestsUrl}/GetAllLendBookRequestsByUserName?username={user.UserName}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiLendBookRequestsUrl}/GetAllLendBookRequestsByUserName?username={user.UserName}");
                if (Res.IsSuccessStatusCode)
                {
                    var LendBookRequestsResponse = Res.Content.ReadAsStringAsync().Result;
                    request = JsonConvert.DeserializeObject<List<LendBookRequests>>(LendBookRequestsResponse);

                    request = request.Where(u => u.Status == "Accepted").ToList();
                    if (request.Count == 0)
                    {
                        TempData["error"] = "You have not borrowed any book or Your borrow request is Pending!";
                        return RedirectToAction("Index", "Home");
                    }
                    return View(request);
                }
                return NotFound();
            }
        }

        
        
        public IActionResult PaymentPage(int TotalAmount,int OrderId)
        {
            ViewBag.TotalAmount = TotalAmount;
            ViewBag.OrderId = OrderId;
            return View();
        } 
        public IActionResult ConfirmationPage(int TotalAmount,int OrderId)
        {
            ViewBag.paidAmount = TotalAmount;
            return View(OrderId);
        }

    }
}
