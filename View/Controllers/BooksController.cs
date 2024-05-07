using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace View.Controllers;

[Route("books")]
public class BooksController : Controller
{
    
    public record LoginReqModel(
        string Username,
        [DataType(DataType.Password)] 
        string Password
    );

    [HttpGet("add")]
    public async Task<IActionResult> BookAdd(int? bookId = -1)
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

                return View(new BookModel
                {
                    Id = bookId.Value,
                    Name = name.FirstOrDefault() ?? bookModel.Name,
                    Author = author.FirstOrDefault() ?? bookModel.Author
                });
            }

            return RedirectToAction("Index");
        }
        else
        {
            var res = await client.GetAsync($"api/book/get/{bookId}");
            var response = res.Content.ReadAsStringAsync().Result;
            bookModel = JsonConvert.DeserializeObject<BookModel>(response);
        }
        
        return Ok(bookModel);
    }
    
    [HttpGet("{start}/{count}")]
    [HttpGet("index")]
    public async Task<List<BookModel>> GetBooks([FromRoute]int start = 0, [FromRoute] int count = -1)
    {
        
        using var client = CreateClient();

        if (HttpContext.User.Identity is { IsAuthenticated: false })
        {
            // var createdAtActionResult = CreatedAtAction(nameof(GetBooks), new LoginReqModel("m", "b"));
            
            // await client.PostAsync("/account/login", new StreamContent())
            var httpResponseMessage = await client.GetAsync("/account/login?returnUrl=/");
            
            // var loginResponse = await client.PostAsJsonAsync("/account/login/?returnUrl=/", new LoginReqModel("m", "b"));

            var responseCookies = httpResponseMessage;
            
            var requestCookie = Request.Cookies["usr"];

            // var user = Request.Cookies["usr"];
            // if (user != null)
            // {
            //     // client.DefaultRequestHeaders = Response.Cookies
            //     // cookieContainer.Add(uri, new Cookie("usr", user));
            // }
        }
        

        var res = await client.GetAsync($"/api/book/select/-1/{start}/{count}");
        var bookModels = new List<BookModel>();
        if (!res.IsSuccessStatusCode)
        {
            /*var errorCodeModel = JsonConvert.DeserializeObject<ErrorCodeModel>(res.Content.ReadAsStringAsync().Result);

            if (errorCodeModel != null)
            {
                ViewData["errorMessage"] = errorCodeModel.Message;
            }
            
            return View(bookModels);*/
        }

        var response = res.Content.ReadAsStringAsync().Result;
        bookModels = JsonConvert.DeserializeObject<List<BookModel>>(response);
        
        return bookModels;
    }
    
    private HttpClient CreateClient()
    {
        var uri = new Uri("https://localhost:7227");
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler()
        {
            
        };
        // handler.CookieContainer = cookieContainer;
        var client = new HttpClient(handler);
        client.BaseAddress = uri;
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        
        var user = Request.Cookies["usr"];
        
        // if (user != null)
        // {
        //     cookieContainer.Add(uri, new Cookie("usr", user));
        // }

        return client;
    }
    
    // public IActionResult Write(string key, string value, bool isPersistent)
    // {
    //     CookieOptions options = new CookieOptions();
    //     if (isPersistent)
    //     {
    //         options.Expires = DateTime.Now.AddDays(1);
    //     }
    //     else
    //     {
    //         options.Expires = DateTime.Now.AddSeconds(5);
    //     }
    //     HttpContext.Response.Cookies.Append(key, value, options);
    // }
}