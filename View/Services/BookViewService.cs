using Contracts.Models;
using View.Data;

namespace View.Services;

/// <summary>
/// Service responsible for correct display of the books on the page.
/// </summary>
public class BookViewService : IListViewService<BookModel>
{
    /// <summary>
    /// Books service.
    /// </summary>
    protected readonly BooksService BooksService;

    /// <summary>
    /// State of the pagination.
    /// </summary>
    public PaginationState PaginationState { get; } = new();
    
    /// <summary>
    /// All available items.
    /// </summary>
    public IEnumerable<BookModel> Items { get; private set; } = Enumerable.Empty<BookModel>();
    
    public event EventHandler? ListChanged;
    
    public event EventHandler<int>? PageChanged;

    public BookViewService(BooksService booksService)
    {
        BooksService = booksService;
    }
    
    public void NotifyListChanged(object? sender)
        => ListChanged?.Invoke(sender, EventArgs.Empty);

    public async void NotifyPageChanged(object? sender, int page)
    {
        PaginationState.SetPage(page);
        
        await GetItemsAsync();
            
        PageChanged?.Invoke(sender, page);
    }
    
    private async ValueTask GetItemsAsync()
    {
        var result = await BooksService.GetAllBooks(PaginationState.BookState, PaginationState.StartWindow, PaginationState.PageSize);
        if (result.Successful)
        {
            Items = result.Items;
            PaginationState.TotalCount = result.TotalCount;
            NotifyListChanged(this);
        }
    }
}