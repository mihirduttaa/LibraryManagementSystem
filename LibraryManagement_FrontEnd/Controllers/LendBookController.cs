using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryManagement_FrontEnd.Controllers
{
    public class LendBookController : Controller
    {
       
        private readonly IConfiguration _Configure;
        private readonly string apiBooksUrl;
        private readonly ApplicationDbContext _db;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string apiLendBookRequestsUrl;

        public LendBookController(IConfiguration configuration, ApplicationDbContext db,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _Configure = configuration;
            apiBooksUrl = _Configure.GetValue<string>("WebAPIBooksUrl");
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            apiLendBookRequestsUrl = _Configure.GetValue<string>("WebAPILendBookRequestsUrl");
        }
        public IActionResult Index()
        {  
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LendBookRequest(int id)
        {
            BooksModel book = new BooksModel();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri($"{apiBooksUrl}/GetbookById?id={id}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiBooksUrl}/GetbookById?id={id}");
                if (Res.IsSuccessStatusCode)
                {
                    var BookResponse = Res.Content.ReadAsStringAsync().Result;
                    book = JsonConvert.DeserializeObject<BooksModel>(BookResponse);
                }

            }
            var user = await _userManager.GetUserAsync(User);
            LendBookRequests request = new LendBookRequests() { BookName=book.Title,CustomerUserName=user.UserName};
            var userFromDB = _db.LendRequests.FirstOrDefault(u =>
            
                 u.BookName == request.BookName && u.CustomerUserName == request.CustomerUserName
           );
            if (userFromDB != null) {
                TempData["Error"] = "Borrow Request already sent to Librarian!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _db.LendRequests.Add(request);
                _db.SaveChanges();
                TempData["title"] = "Request sent SuccessFully";
                TempData["success"] = "You can collect the book once request is Accepted. Please Check Request status in Borrow Request Section";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcceptLendBookStatus(int id)
        {
            if(id!=null || id != 0)
            {
                LendBookRequests RequestFromDB = null;
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri($"{apiLendBookRequestsUrl}/GetLendBookRequestById?id={id}");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync($"{apiLendBookRequestsUrl}/GetLendBookRequestById?id={id}");
                    if (Res.IsSuccessStatusCode)
                    {
                        var LendBookRequestsResponse = Res.Content.ReadAsStringAsync().Result;
                        RequestFromDB = JsonConvert.DeserializeObject<LendBookRequests>(LendBookRequestsResponse);

                    }

                }
                if (RequestFromDB != null)
                {
                    if (RequestFromDB.Status == "Rejected")
                    {
                        return Json(new { success = false, message = "Cannot Accept Rejected Lend Request" });
                    }
                    RequestFromDB.Status = "Accepted";
                    _db.LendRequests.Update(RequestFromDB);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Lend Requested Accepted Successfully" });
                }
                
            }
            return Json(new { success = false, message = "Error while Accepting Lend Request" });
        }

        [HttpPost]
        public async Task<IActionResult> RejectLendBookStatus(int id)
        {
            if (id != null || id != 0)
            {
                LendBookRequests RequestFromDB=null;
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri($"{apiLendBookRequestsUrl}/GetLendBookRequestById?id={id}");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync($"{apiLendBookRequestsUrl}/GetLendBookRequestById?id={id}");
                    if (Res.IsSuccessStatusCode)
                    {
                        var LendBookRequestsResponse = Res.Content.ReadAsStringAsync().Result;
                        RequestFromDB = JsonConvert.DeserializeObject<LendBookRequests>(LendBookRequestsResponse);

                    }
                   
                }
                if (RequestFromDB != null)
                {
                    if (RequestFromDB.Status == "Accepted")
                    {
                        return Json(new { success = false, message = "Cannot Reject Accepted Lend Request" });
                    }
                    RequestFromDB.Status = "Rejected";
                    _db.LendRequests.Update(RequestFromDB);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Lend Requested Rejected Successfully" });
                }

            }
            return Json(new { success = false, message = "Error while Rejecting Lend Request" });
        }

        [HttpGet]
        public async Task<IActionResult> ReturnBook(int id)
        {
            LendBookRequests request = new LendBookRequests();
            using (var client = new HttpClient())
            {
                
                client.BaseAddress = new Uri($"{apiLendBookRequestsUrl}/GetLendBookRequestById?id={id}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiLendBookRequestsUrl}/GetLendBookRequestById?id={id}");
                if (Res.IsSuccessStatusCode)
                {
                    var LendBookRequestsResponse = Res.Content.ReadAsStringAsync().Result;
                    request = JsonConvert.DeserializeObject<LendBookRequests>(LendBookRequestsResponse);
                  
                }
                return View(request);
            }
        }
        [ActionName("ReturnBook")]
        [HttpPost]
        public async Task<IActionResult> ReturnBookPost(int id)
        {
            using (HttpClient client = new HttpClient())
            {
               
                string endpoint = apiLendBookRequestsUrl + $"/DeleteLendBookRequestById?id={id}";
                using (var Response = await client.DeleteAsync(endpoint))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        TempData["success"] = "Book Returned SuccessFully";
                        return RedirectToAction("Index", "LendBook");
                    }
                    else
                    {
                        TempData["error"] = "Book Return Operation Failed!";
                        return View();
                    }
                }
            }
          
        }
    }
}
