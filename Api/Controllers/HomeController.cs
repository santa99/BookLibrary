using System.Net.Http.Headers;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

public class HomeController : Controller
{
    private bool IsLoggedIn()
    {
        return Request.Cookies["user"] != null;
    }

    [Route("/")]
    public async Task<IActionResult> Index()
    {
        var user = Request.Cookies["user"];
        if (user != null)
        {
            ViewBag.User = user;
        }

        var bookModels = new List<BookModel>();

        if (!IsLoggedIn())
        {
            return View(bookModels);
        }


        using var client = new HttpClient();
        
        client.BaseAddress = new Uri("https://localhost:7227");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var res = await client.GetAsync("api/book/select/-1");
        if (!res.IsSuccessStatusCode)
        {
            return View(bookModels);
        }
            
        var response = res.Content.ReadAsStringAsync().Result;
        bookModels = JsonConvert.DeserializeObject<List<BookModel>>(response);

        return View(bookModels);
    }
}