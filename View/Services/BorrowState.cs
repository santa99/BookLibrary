using Contracts.Models;

namespace View.Services;

public class BorrowState
{
    private readonly BooksService _booksService;

    /// <summary>
    /// Dialog is being displayed when true.
    /// </summary>
    public bool ShowingConfigureDialog { get; private set; }
    
    /// <summary>
    /// Currently configured book.
    /// </summary>
    public BookModel CurrentBook { get; private set; }

    public BorrowState(BooksService booksService)
    {
        _booksService = booksService;
    }

    /// <summary>
    /// Show configuration dialog over the book.
    /// </summary>
    /// <param name="bookModel"></param>
    public void ShowConfigureDialog(BookModel bookModel)
    {
        CurrentBook = new BookModel()
        {
            Author = bookModel.Author,
            Borrowed = bookModel.Borrowed,
            Id = bookModel.Id,
            Name = bookModel.Name
            
        };
        
        ShowingConfigureDialog = true;
    }

    public void CancelConfigureDialog()
    {
        CurrentBook = null;

        ShowingConfigureDialog = false;
    }

    public async Task BorrowBook(int readersCardId)
    {
        await _booksService.BorrowBook(CurrentBook.Id, readersCardId);
        CurrentBook = null;
        ShowingConfigureDialog = false;
    }

    public async Task ReturnBook(int bookId)
    {
        await _booksService.ReturnBook(bookId);
        CurrentBook = null;
        ShowingConfigureDialog = false;
    }
}