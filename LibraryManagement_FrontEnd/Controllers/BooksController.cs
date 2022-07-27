using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryManagement_FrontEnd.Controllers
{
    public class BooksController : Controller
    {
        private IConfiguration _Configure;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string apiBooksUrl;
        private readonly ApplicationDbContext _db;
        public BooksController(IConfiguration configuration, IWebHostEnvironment hostEnvironment,ApplicationDbContext db)
        {
            _Configure = configuration;
            _hostEnvironment = hostEnvironment;
            apiBooksUrl = _Configure.GetValue<string>("WebAPIBooksUrl");
            _db = db;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpsertBook(int? id)
        {
            if (id == null || id == 0)
            {
                BooksModel book = new BooksModel();
                return View(book);
            }
            else
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
                    return View(book);
                }
                return View(book);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpsertBook(BooksModel obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\books");
                    var extention = Path.GetExtension(file.FileName);

                    if (obj.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extention), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.ImageUrl = @"\images\books\" + fileName + extention;
                }
                if (obj.Id == 0 || obj.Id == null)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        string endpoint = apiBooksUrl + "/AddBook";
                        using (var Response = await client.PostAsync(endpoint, content))
                        {
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                TempData["success"] = "Book Added SuccessFully";
                                return RedirectToAction("Index", "Books");
                            }
                            else
                            {
                                TempData["error"] = "Book Add Operation Failed!";
                                return View();
                            }
                        }
                    }

                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        string endpoint = apiBooksUrl + "/UpdateBook";
                        using (var Response = await client.PutAsync(endpoint, content))
                        {
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                TempData["success"] = "Book Updated SuccessFully";
                                return RedirectToAction("Index", "Books");
                            }
                            else
                            {
                                TempData["error"] = "Book  Updation Failed!";
                                return View();
                            }
                        }
                    }

                }
                
            }
            return View(obj);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var obj = _db.Books.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                string endpoint = apiBooksUrl+$"/DeleteBook?id={id}";
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

    }
}
