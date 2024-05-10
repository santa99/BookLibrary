using Contracts.Models;

namespace View.Services;

public class BorrowState
{
    private readonly BooksService _booksService;
    public EventHandler<BookModel> BookModelUpdated { get; set; }
    public bool ShowingBookConfigurationDialog { get; private set; }
    public BookModel? CurrentBook { get; private set; }

    public BorrowState(BooksService booksService)
    {
        _booksService = booksService;
        BookModelUpdated = (_, _) => {};
    }

    /// <summary>
    /// Call this method to show configuration dialog over the book.
    /// </summary>
    /// <param name="bookModel">Book model to borrow or return.</param>
    public void ShowConfigureDialog(BookModel bookModel)
    {
        if (ShowingBookConfigurationDialog)
        {
            return;
        }
        
        CurrentBook = new BookModel
        {
            Author = bookModel.Author,
            Borrowed = bookModel.Borrowed,
            Id = bookModel.Id,
            Name = bookModel.Name
        };

        ShowingBookConfigurationDialog = true;
    }

    /// <summary>
    /// Call this method if you want to close dialog.
    /// </summary>
    public void CancelConfigureDialog()
    {
        CurrentBook = null;

        ShowingBookConfigurationDialog = false;
    }

    /// <summary>
    /// Action delegate called when given book is about to borrow.
    /// </summary>
    /// <param name="readersCardId">Readers card id.</param>
    public async Task BorrowBook(int readersCardId)
    {
        if (CurrentBook == null)
        {
            return;
        }
        
        var borrowModel = await _booksService.BorrowBook(CurrentBook.Id, readersCardId);
        
        BookModelUpdated.Invoke(this, new BookModel
        {
            Id = CurrentBook.Id,
            Name = CurrentBook.Name,
            Author = CurrentBook.Author,
            Borrowed = borrowModel
        });
        
        CurrentBook = null;
        ShowingBookConfigurationDialog = false;
    }

    /// <summary>
    /// Action delegate called when the currently configured book is about to be returned.
    /// </summary>
    /// <param name="bookId">Book id.</param>
    public async Task ReturnBook(int bookId)
    {
        if (CurrentBook == null)
        {
            return;
        }
        
        var returnedBook = await _booksService.ReturnBook(bookId);
        if (bookId != returnedBook)
        {
            return;
        }

        BookModelUpdated.Invoke(this, new BookModel
        {
            Id = CurrentBook.Id,
            Name = CurrentBook.Name,
            Author = CurrentBook.Author,
            Borrowed = null
        });
        
        CurrentBook = null;
        ShowingBookConfigurationDialog = false;
    }
}