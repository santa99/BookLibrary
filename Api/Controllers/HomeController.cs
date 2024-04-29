using System.Net;
using System.Net.Http.Headers;
using Contracts.Models;
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
        if (!IsLoggedIn())
        {
            return View(null);
        }
        ViewBag.User = Request.Cookies["user"] ?? "";
        
        using var client = CreateClient();

        var res = await client.GetAsync("api/book/select/-1");
        var bookModels = new List<BookModel>();
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
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index");
        }

        var bookModel = new BookModel();

        using var client = CreateClient();

        if (HttpContext.Request.Method == HttpMethod.Post.Method)
        {
            var name = HttpContext.Request.Form["Name"];
            var author = HttpContext.Request.Form["Author"];
            var queryString = bookId != -1 
                ? $"api/book/edit/{bookId}?title={name.FirstOrDefault()}&author={author.FirstOrDefault()}"
                : $"api/book/add?title={name.FirstOrDefault()}&author={author.FirstOrDefault()}";
            var res = await client.GetAsync(queryString);
            
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
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index");
        }

        using var client = CreateClient();
        
        await client.GetAsync($"api/book/remove/{bookId}");
        
        return RedirectToAction("Index");
    }
    
    [Route("/home/return/{bookId}")]
    public async Task<IActionResult> ReturnBook(int bookId)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index");
        }

        using var client = CreateClient();
        
        await client.GetAsync($"api/book/return/{bookId}");
        
        return RedirectToAction("Index");
    }

    [Route("/home/borrow/{bookId}")]
    public async Task<IActionResult> BorrowBook(int bookId)
    {
        if (!IsLoggedIn())
        {
            return RedirectToAction("Index");
        }

        using var client = CreateClient();

        if (HttpContext.Request.Method == HttpMethod.Post.Method)
        {
            var readersCardId =  HttpContext.Request.Form["ReadersCardId"].FirstOrDefault();
            await client.GetAsync($"api/book/borrow/{bookId}/{readersCardId}");
            
            return RedirectToAction("Index");
        }

        var bookResponse = await client.GetAsync($"api/book/get/{bookId}");
        var readersResponse = await client.GetAsync("api/readers/select/");

        var bookModel = JsonConvert.DeserializeObject<BookModel>(bookResponse.Content.ReadAsStringAsync().Result);
        var readers =
            JsonConvert.DeserializeObject<List<ReadersInfo>>(readersResponse.Content.ReadAsStringAsync().Result);
        ViewData["Readers"] = readers;
        return View(bookModel);
    }

    private HttpClient CreateClient()
    {
        var uri = new Uri("https://localhost:7227");
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        var client = new HttpClient(handler);
        client.BaseAddress = uri;
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var user = Request.Cookies["user"];
        if (user != null)
        {
            cookieContainer.Add(uri, new Cookie("user", user));
        }

        return client;
    }


}