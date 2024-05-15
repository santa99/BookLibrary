using System.Net;
using System.Net.Http.Headers;
using Api.Models.Responses;
using Contracts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

/// <summary>
/// Note: This class is not cleaned up.
/// - We could use Refit to create Client rather than doing it manually.
/// </summary>
[Authorize]
public class HomeController : Controller
{
    
    [Authorize]
    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var httpContextUser = HttpContext.User;
        if (httpContextUser.Identity?.Name != null)
        {
            ViewBag.User = httpContextUser.Identity.Name;
        }

        using var client = CreateClient();

        var res = await client.GetAsync("/api/book/select/-1");
        var bookModels = new List<BookModel>();
        if (!res.IsSuccessStatusCode)
        {
            var errorCodeModel = JsonConvert.DeserializeObject<ErrorCodeModel>(res.Content.ReadAsStringAsync().Result);

            if (errorCodeModel != null)
            {
                ViewData["errorMessage"] = errorCodeModel.Message;
            }
            
            return View(bookModels);
        }

        var response = res.Content.ReadAsStringAsync().Result;
        bookModels = JsonConvert.DeserializeObject<List<BookModel>>(response);

        return View(bookModels);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bookId"></param>
    /// <returns></returns>
    [HttpGet("home/edit/{bookId}")]
    [HttpPost("home/edit/{bookId}")] //Maybe put?
    public async Task<IActionResult> EditBook([FromRoute] int bookId)
    {

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
                var errorCodeModel = JsonConvert.DeserializeObject<ErrorCodeModel>(res.Content.ReadAsStringAsync().Result);

                if (errorCodeModel != null)
                {
                    ViewData["errorMessage"] = errorCodeModel.Message;
                }                
                return View(new BookModel
                {
                    Id = bookId,
                    Name = name.FirstOrDefault() ?? bookModel.Name,
                    Author = author.FirstOrDefault() ?? bookModel.Author
                });
            }

            return RedirectToAction("Index");
        }
        else
        {
            var res = await client.GetAsync($"api/book/get/{bookId}");
            if (!res.IsSuccessStatusCode)
            {
                var errorCodeModel = JsonConvert.DeserializeObject<ErrorCodeModel>(res.Content.ReadAsStringAsync().Result);

                if (errorCodeModel != null)
                {
                    ViewData["errorMessage"] = errorCodeModel.Message;
                }                
                return View(bookModel);
            }

            var response = res.Content.ReadAsStringAsync().Result;
            bookModel = JsonConvert.DeserializeObject<BookModel>(response);

            return View(bookModel);
        }
    }

    [HttpDelete("home/remove/{bookId}")]
    public async Task<IActionResult> RemoveBook(int bookId)
    {
        using var client = CreateClient();
        
        await client.GetAsync($"api/book/remove/{bookId}");
        
        return RedirectToAction("Index");
    }
    
    [HttpGet("/home/return/{bookId}")]
    public async Task<IActionResult> ReturnBook(int bookId)
    {
        using var client = CreateClient();
        
        await client.GetAsync($"api/book/return/{bookId}");
        
        return RedirectToAction("Index");
    }

    [HttpGet("/home/borrow/{bookId}")]
    [HttpPost("/home/borrow/{bookId}")]
    public async Task<IActionResult> BorrowBook(int bookId)
    {
        using var client = CreateClient();

        if (HttpContext.Request.Method == HttpMethod.Post.Method)
        {
            var dateTimeOffset = DateTimeOffset.Now;
            var readersCardId =  HttpContext.Request.Form["ReadersCardId"].FirstOrDefault();
            await client.GetAsync($"api/book/borrow/{bookId}/{readersCardId}?From={dateTimeOffset.Date}");
            
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
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var user = Request.Cookies["usr"];
        if (user != null)
        {
            cookieContainer.Add(uri, new Cookie("usr", user));
        }

        return client;
    }


}