using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryManagement_FrontEnd.Controllers
{
    public class AuthorsController : Controller
    {
        private IConfiguration _Configure;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string apiAuthorsUrl;
        private readonly string apiBooksUrl;

        public AuthorsController(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _Configure = configuration;
            _hostEnvironment = hostEnvironment;
            apiAuthorsUrl = _Configure.GetValue<string>("WebAPIAuthorsUrl");
            apiBooksUrl = _Configure.GetValue<string>("WebAPIBooksUrl");
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            AuthorModel author = new AuthorModel();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri($"{apiAuthorsUrl}/GetAuthorById?id={id}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiAuthorsUrl}/GetAuthorById?id={id}");
                if (Res.IsSuccessStatusCode)
                {
                    var AuthorResponse = Res.Content.ReadAsStringAsync().Result;
                    author = JsonConvert.DeserializeObject<AuthorModel>(AuthorResponse);
                }
                
            }
            List<BooksModel> AllBooks = new List<BooksModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBooksUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiBooksUrl}/GetAllBooks");
                if (Res.IsSuccessStatusCode)
                {
                    var BooksResponse = Res.Content.ReadAsStringAsync().Result;
                    AllBooks = JsonConvert.DeserializeObject<List<BooksModel>>(BooksResponse);
                }

            
            }
            AllBooks = AllBooks.Where(u => u.Author.ToLower() == author.Name.ToLower()).ToList();
            AuthorVM AuthorObj = new()
            {

                Author = author,
                Books = AllBooks

            };

            return View(AuthorObj);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertAuthor(int? id)
        {
            if (id == null || id == 0)
            {
                AuthorModel author = new AuthorModel();
                return View(author);
            }
            else
            {
                AuthorModel author = new AuthorModel();
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri($"{apiAuthorsUrl}/GetAuthorById?id={id}");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync($"{apiAuthorsUrl}/GetAuthorById?id={id}");
                    if (Res.IsSuccessStatusCode)
                    {
                        var AuthorResponse = Res.Content.ReadAsStringAsync().Result;
                        author = JsonConvert.DeserializeObject<AuthorModel>(AuthorResponse);
                    }
                    return View(author);
                }
                return View(author);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpsertAuthor(AuthorModel obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\authors");
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
                    obj.ImageUrl = @"\images\authors\" + fileName + extention;
                }
                if (obj.Id == 0 || obj.Id == null)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        string endpoint = apiAuthorsUrl + "/AddAuthor";
                        using (var Response = await client.PostAsync(endpoint, content))
                        {
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                TempData["success"] = "Author Added SuccessFully";
                                return RedirectToAction("Index", "Authors");
                            }
                            else
                            {
                                TempData["error"] = "Author Add Operation Failed!";
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
                        string endpoint = apiAuthorsUrl + "/UpdateAuthor";
                        using (var Response = await client.PutAsync(endpoint, content))
                        {
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                TempData["success"] = "Author Updated SuccessFully";
                                return RedirectToAction("Index", "Authors");
                            }
                            else
                            {
                                TempData["error"] = "Author  Update Operation Failed!";
                                return View();
                            }
                        }
                    }

                }

            }
            return View(obj);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            AuthorModel author = new AuthorModel();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri($"{apiAuthorsUrl}/GetAuthorById?id={id}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiAuthorsUrl}/GetAuthorById?id={id}");
                if (Res.IsSuccessStatusCode)
                {
                    var AuthorResponse = Res.Content.ReadAsStringAsync().Result;
                    author = JsonConvert.DeserializeObject<AuthorModel>(AuthorResponse);
                }
            }
            var obj = author;
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
                string endpoint = apiAuthorsUrl + $"/DeleteAuthor?id={id}";
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
