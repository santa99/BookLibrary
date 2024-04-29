using System.Net;
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


        var uri = new Uri("https://localhost:7227");

        var cookieContainer = new CookieContainer();

        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        using var client = new HttpClient(handler);

        client.BaseAddress = uri;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        cookieContainer.Add(uri, new Cookie("user", user));

        var res = await client.GetAsync("api/book/select/-1");
        if (!res.IsSuccessStatusCode)
        {
            return View(bookModels);
        }

        var response = res.Content.ReadAsStringAsync().Result;
        bookModels = JsonConvert.DeserializeObject<List<BookModel>>(response);

        return View(bookModels);
    }

    [Route("home/edit/{bookId}")]
    public async Task<IActionResult> EditBook([FromRoute] int bookId)
    {
        var user = Request.Cookies["user"];
        if (user != null)
        {
            ViewBag.User = user;
        }

        var bookModel = new BookModel();
        
        if (!IsLoggedIn())
        {
            return View(bookModel);
        }

        var uri = new Uri("https://localhost:7227");
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        using var client = new HttpClient(handler);
        client.BaseAddress = uri;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        cookieContainer.Add(uri, new Cookie("user", user));


        if (HttpContext.Request.Method == HttpMethod.Post.Method)
        {
            var titleVals = HttpContext.Request.Form["Name"];
            var authorVals = HttpContext.Request.Form["Author"];
            var res = await client.GetAsync($"api/book/edit/{bookId}?title={titleVals.FirstOrDefault()}&author={authorVals.FirstOrDefault()}");
            if (!res.IsSuccessStatusCode)
            {
                return View(bookModel);
            }
            
            return RedirectToAction("Index");
        }
        else
        {
            var res = await client.GetAsync($"api/book/get/{bookId}");
            if (!res.IsSuccessStatusCode)
            {
                return View(bookModel);
            }

            var response = res.Content.ReadAsStringAsync().Result;
            bookModel = JsonConvert.DeserializeObject<BookModel>(response);


            return View(bookModel);
        }
    }

    [Route("home/remove/{bookId}")]
    public async Task<IActionResult> RemoveBook(int bookId)
    {
        var user = Request.Cookies["user"];
        if (user != null)
        {
            ViewBag.User = user;
        }

        if (!IsLoggedIn())
        {
            return RedirectToAction("Index");
        }

        var uri = new Uri("https://localhost:7227");
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        using var client = new HttpClient(handler);
        client.BaseAddress = uri;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        cookieContainer.Add(uri, new Cookie("user", user));

        await client.GetAsync($"api/book/remove/{bookId}");
        return RedirectToAction("Index");
    }
}