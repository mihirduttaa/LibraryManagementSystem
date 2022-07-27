using LibraryManagement_FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryManagement_FrontEnd.Controllers
{
    public class PublishersController : Controller
    {
        private IConfiguration _Configure;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string apiPublishersUrl;
        private readonly string apiBooksUrl;

        public PublishersController(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _Configure = configuration;
            _hostEnvironment = hostEnvironment;
            apiPublishersUrl = _Configure.GetValue<string>("WebAPIPublishersUrl");
            apiBooksUrl = _Configure.GetValue<string>("WebAPIBooksUrl");
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            PublisherModel publisher = new PublisherModel();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri($"{apiPublishersUrl}/GetPublisherById?id={id}");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync($"{apiPublishersUrl}/GetPublisherById?id={id}");
                if (Res.IsSuccessStatusCode)
                {
                    var PublisherResponse = Res.Content.ReadAsStringAsync().Result;
                    publisher = JsonConvert.DeserializeObject<PublisherModel>(PublisherResponse);
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
            AllBooks = AllBooks.Where(u => u.Publisher.ToLower() == publisher.Name.ToLower()).ToList();
            PublisherVM PublisherObj = new()
            {

                Publisher = publisher,
                Books = AllBooks

            };

            return View(PublisherObj);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertPublisher(int? id)
        {
            if (id == null || id == 0)
            {
                PublisherModel publisher = new PublisherModel();
                return View(publisher);
            }
            else
            {
                PublisherModel publisher = new PublisherModel();
                using (var client = new HttpClient())
                {

                    client.BaseAddress = new Uri($"{apiPublishersUrl}/GetPublisherById?id={id}");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync($"{apiPublishersUrl}/GetPublisherById?id={id}");
                    if (Res.IsSuccessStatusCode)
                    {
                        var AuthorResponse = Res.Content.ReadAsStringAsync().Result;
                        publisher = JsonConvert.DeserializeObject<PublisherModel>(AuthorResponse);
                    }
                    return View(publisher);
                }
                return View(publisher);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpsertPublisher(AuthorModel obj)
        {
            if (ModelState.IsValid)
            { 
                if (obj.Id == 0 || obj.Id == null)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        string endpoint = apiPublishersUrl + "/AddPublisher";
                        using (var Response = await client.PostAsync(endpoint, content))
                        {
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                TempData["success"] = "Publisher Added SuccessFully";
                                return RedirectToAction("Index", "Publishers");
                            }
                            else
                            {
                                TempData["error"] = "Publisher Add Operation Failed!";
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
                        string endpoint = apiPublishersUrl + "/UpdatePublisher";
                        using (var Response = await client.PutAsync(endpoint, content))
                        {
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                TempData["success"] = "Publisher Updated SuccessFully";
                                return RedirectToAction("Index", "Publishers");
                            }
                            else
                            {
                                TempData["error"] = "Publisher  Updation Failed!";
                                return View();
                            }
                        }
                    }

                }

            }
            return View(obj);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                string endpoint = apiPublishersUrl + $"/DeletePublisher?id={id}";
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
