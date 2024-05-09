﻿using Microsoft.AspNetCore.Components;

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

    public async Task BorrowBook(int bookId, int readersCardId)
    {
        using var client = _httpClientFactory.CreateClient();
        
        await client.GetAsync(_navigationManager.BaseUri + $"books/borrow/{bookId}/{readersCardId}");
    }

    public async Task ReturnBook(int bookId)
    {
        using var client = _httpClientFactory.CreateClient();
        
        await client.GetAsync(_navigationManager.BaseUri + $"books/return/{bookId}");
    }
}