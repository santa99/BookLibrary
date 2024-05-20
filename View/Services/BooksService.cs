using System.Net;
using Contracts.Exceptions;
using Contracts.Models;
using Contracts.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using View.Model;

namespace View.Services;

public class BooksService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BooksService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<BorrowModel?> BorrowBook(int bookId, int readersCardId)
    {
        using var client = CreateClient();

        var borrowModel = await client.GetFromJsonAsync<BorrowModel>($"api/book/borrow/{bookId}/{readersCardId}");

        return borrowModel;
    }

    public async Task<int> ReturnBook(int bookId)
    {
        using var client = CreateClient();

        var result = await client.GetFromJsonAsync<int>($"api/book/return/{bookId}");

        if (result != bookId)
        {
            return -1;
        }
        
        return bookId;
    }

    public async Task<BookModel?> GetBook(int bookId)
    {
        using var client = CreateClient();

        var book =  await client.GetFromJsonAsync<BookModel>(
            $"api/book/get/{bookId}");

        return book;
    }

    public async Task<List<BookModel>> GetBooks(int start = 0, int count = -1)
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

    public async Task<IActionResult?> UpdateBook(BookModel bookModel)
    {
        using var client = CreateClient();
        
        var updateBookReqModel = new UpdateBookReqModel(bookModel.Id < 0 ? null : bookModel.Id, bookModel.Name, bookModel.Author);

        var queryStr = "";
        queryStr = updateBookReqModel.BookId == null 
            ? $"/api/book/add?title={updateBookReqModel.Title}&author={updateBookReqModel.Author}" 
            : $"/api/book/edit/{updateBookReqModel.BookId}?title={updateBookReqModel.Title}&author={updateBookReqModel.Author}";

        var result = await client.GetAsync(queryStr);
        var content = await result.Content.ReadAsStringAsync();

        return result.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(JsonConvert.DeserializeObject<BookModel>(content)),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(
                JsonConvert.DeserializeObject<ErrorCodeModel>(content)),
            _ => null
        };
    }

    public async Task RemoveBook(BookModel bookModel)
    {
        using var client = CreateClient();
        
        await client.DeleteAsync($"/api/book/remove/{bookModel.Id}");
    }
    
    private HttpClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://localhost:7227");
        return httpClient;
    }
}

