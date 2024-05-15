using System.ComponentModel.DataAnnotations;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using View.Model;

namespace View.Controllers;

[Route("books")]
[ApiController]
public class BooksController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BooksController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public record LoginReqModel(
        string Username,
        [DataType(DataType.Password)] 
        string Password
    );

    [HttpGet("borrow/{bookId}/{readersCardId}")]
    public async Task<IActionResult> BorrowBook(int bookId, int readersCardId)
    {
        using var client = CreateClient();
        
        var httpResponseMessage = await client.GetFromJsonAsync<BorrowModel>($"/api/book/borrow/{bookId}/{readersCardId}");
        if (httpResponseMessage == null)
        {
            return Forbid();
        }
        
        return Ok(httpResponseMessage);
    }

    [HttpGet("return/{bookId}/")]
    public async Task<IActionResult> ReturnBook(int bookId)
    {
        using var client = CreateClient();
        
        var httpResponseMessage = await client.GetAsync($"/api/book/return/{bookId}");
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return Forbid();
        }
        
        return Ok();
    }
    
    [HttpPost("update")]
    public async Task<BookModel> UpdateBook([FromBody] UpdateBookReqModel updateBookReqModel)
    {
        using var client = CreateClient();
        
        var bookId = updateBookReqModel?.BookId;
        var title = updateBookReqModel.Title;
        var author = updateBookReqModel.Author;

        var queryStr = "";
        queryStr = bookId == null 
            ? $"/api/book/add?title={title}&author={author}" 
            : $"/api/book/edit/{bookId}?title={title}&author={author}";
        
        var httpResponseMessage = await client.GetFromJsonAsync<BookModel>(queryStr);
        
        return httpResponseMessage;
    }

    [HttpGet("remove/{bookId}/")]
    public async Task<IActionResult> RemoveBook(int? bookId = -1)
    {
        using var client = CreateClient();
        
        var httpResponseMessage = await client.GetAsync($"/api/book/remove/{bookId}");
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return Forbid();
        }
        
        return Ok();
    }

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
    
    [HttpGet("get/{bookId}/")]
    public async Task<BookModel?> GetBook(int bookId)
    {
        using var client = CreateClient();
        
        var book = await client.GetFromJsonAsync<BookModel>($"/api/book/get/{bookId}");

        return book;
    }
    
    [HttpGet("{start}/{count}")]
    [HttpGet("index")]
    public async Task<List<BookModel>> GetBooks([FromRoute]int start = 0, [FromRoute] int count = -1)
    {
        using var client = CreateClient();
        
        var res = await client.GetAsync($"/api/book/select/-1/{start}/{count}");
        var bookModels = new List<BookModel>();

        var response = res.Content.ReadAsStringAsync().Result;
        try
        {
            bookModels = JsonConvert.DeserializeObject<List<BookModel>>(response);
        }
        catch (Exception e)
        {
            
        }
        
        return bookModels;
    }

    private HttpClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://localhost:7227");
        return httpClient;
    }

}

