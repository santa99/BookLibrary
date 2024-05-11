using System.ComponentModel.DataAnnotations;
using Contracts.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using View.Model;

namespace View.Services;

public class BooksService
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpClientFactory _httpClientFactory;

    public BooksService(NavigationManager navigationManager, IHttpClientFactory httpClientFactory)
    {
        _navigationManager = navigationManager;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<BorrowModel> BorrowBook(int bookId, int readersCardId)
    {
        using var client = _httpClientFactory.CreateClient();

        var borrowModel = await client.GetFromJsonAsync<BorrowModel>(_navigationManager.BaseUri + $"books/borrow/{bookId}/{readersCardId}");

        return borrowModel;
    }

    public async Task<int> ReturnBook(int bookId)
    {
        using var client = _httpClientFactory.CreateClient();

        await client.GetAsync(_navigationManager.BaseUri + $"books/return/{bookId}");

        return bookId;
    }

    public async Task<BookModel?> GetBook(int bookId)
    {
        using var client = _httpClientFactory.CreateClient();

        var book =  await client.GetFromJsonAsync<BookModel>(
            _navigationManager.BaseUri + $"books/get/{bookId}");

        return book;
    }

    public async Task<List<BookModel>> GetBooks(int start, int count)
    {
        using var client = _httpClientFactory.CreateClient();

        var books = await client.GetFromJsonAsync<List<BookModel>>(
            _navigationManager.BaseUri + $"books/{start}/{count}");

        return books ?? new List<BookModel>();
    }

    public async Task<BookModel?> UpdateBook(BookModel bookModel)
    {
        using var client = _httpClientFactory.CreateClient();
        
        var updateBookReqModel = new UpdateBookReqModel(bookModel.Id < 0 ? null : bookModel.Id, bookModel.Name, bookModel.Author);

        var httpResponseMessage = await client.PostAsync(_navigationManager.BaseUri + $"books/update", 
            JsonContent.Create(updateBookReqModel));

        var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<BookModel>(readAsStringAsync);
    }

    public async Task RemoveBook(BookModel bookModel)
    {
        using var client = _httpClientFactory.CreateClient();
        
        await client.GetAsync(_navigationManager.BaseUri + $"books/remove/{bookModel.Id}");
    }
}

