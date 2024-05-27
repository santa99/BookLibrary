using System.Net;
using Contracts.Models;
using Contracts.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using View.Data;
using View.Exceptions;
using View.Model;

namespace View.Services;

public class BooksService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BooksService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<BorrowModel> BorrowBook(int bookId, int readersCardId)
    {
        using var client = CreateClient();
        var result = await client.PostAsync("api/book/borrow/",
            JsonContent.Create(new CreateBorrowReqModel(bookId, readersCardId, DateTime.UtcNow)));
        var content = await result.Content.ReadAsStringAsync();
        
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return JsonConvert.DeserializeObject<BorrowModel>(content);
        }
        
        var errorCodeModel = JsonConvert.DeserializeObject<ErrorCodeModel>(content);
        if (errorCodeModel is { ClientMessage: not null })
        {
            throw new SpecifiedException(errorCodeModel.ClientMessage);
        }
            
        throw new UnspecifiedException();
    }

    public async Task<int> ReturnBook(int bookId)
    {
        using var client = CreateClient();

        var result = await client.PutAsync($"api/book/return/{bookId}", null);
        var content = await result.Content.ReadAsStringAsync();

        if (result.StatusCode == HttpStatusCode.OK)
        {
            return JsonConvert.DeserializeObject<int>(content);
        }

        var errorCodeModel = JsonConvert.DeserializeObject<ErrorCodeModel>(content);
        if (errorCodeModel is { ClientMessage: not null })
        {
            throw new SpecifiedException(errorCodeModel.ClientMessage);
        }
            
        throw new UnspecifiedException();
    }

    public async Task<IActionResult?> GetBook(int bookId)
    {
        using var client = CreateClient();

        var httpResponse =  await client.GetAsync(
            $"api/book/get/{bookId}");

        var response = httpResponse.Content.ReadAsStringAsync().Result;
        return httpResponse.StatusCode switch
        {
            HttpStatusCode.OK =>
                new OkObjectResult(JsonConvert.DeserializeObject<BookModel>(response)),
            HttpStatusCode.NotFound =>
                new BadRequestObjectResult(JsonConvert.DeserializeObject<ErrorCodeModel>(response)),
            _ => null
        };
    }

    public async ValueTask<ItemsQueryResult<BookModel>> GetAllBooks(int start = 0, int count = -1)
    {
        // TODO: this needs a new endpoint for retrieving only count.
        var totalCount = (await  GetBooks(0, -1)).Count;
        var books = await GetBooks(start, count);

        return new ItemsQueryResult<BookModel>(books, totalCount, true);
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

        var result = updateBookReqModel.BookId == null
                ? await client.PostAsync(queryStr, JsonContent.Create(new CreateBookReqModel(bookModel.Name, bookModel.Author)))
                : await client.PatchAsync(queryStr, null);
        var content = await result.Content.ReadAsStringAsync();

        return result.StatusCode switch
        {
            HttpStatusCode.OK => new OkObjectResult(JsonConvert.DeserializeObject<BookModel>(content)),
            HttpStatusCode.Created => new OkObjectResult(JsonConvert.DeserializeObject<BookModel>(content)),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(
                JsonConvert.DeserializeObject<ErrorCodeModel>(content)),
            _ => null
        };
    }

    public async Task<bool> RemoveBook(BookModel bookModel)
    {
        using var client = CreateClient();
        
        await client.DeleteAsync($"/api/book/remove/{bookModel.Id}");

        return true;
    }
    
    private HttpClient CreateClient()
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://localhost:7227");
        return httpClient;
    }
}
