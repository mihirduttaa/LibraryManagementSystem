using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryManagement_FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _Configure;
        private readonly string apiBooksUrl;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration,ApplicationDbContext db)
        {
            _logger = logger;
            _Configure = configuration;
            apiBooksUrl = _Configure.GetValue<string>("WebAPIBooksUrl");
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> LendBookDetails(int id)
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
            return View(book);
        }
        public async Task<IActionResult> Index(string? search)
        {
            var Books1 = from m in _db.Books
                         select m;

            if (!String.IsNullOrEmpty(search))
            {
                Books1 = Books1.Where(
                    s => s.Title.ToLower().Contains(search.ToLower()) || s.Author.ToLower().Contains(search.ToLower()) || s.Publisher.ToLower().Contains(search.ToLower())
                    
                    );
                if (Books1.Count() != 0)
                {
                    return View(await Books1.ToListAsync());
                }
                TempData["error"] = "Searched Book Not Found!";
            }

            List<BooksModel> Books=new List<BooksModel>();
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBooksUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiBooksUrl}/GetAllBooks");
                if (Res.IsSuccessStatusCode)
                {
                    var BooksResponse = Res.Content.ReadAsStringAsync().Result;
                    Books = JsonConvert.DeserializeObject<List<BooksModel>>(BooksResponse);
                }
               
                return View(Books);
            }
          
        }
        

        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}