using Contracts.Models;
using View.Exceptions;

namespace View.Services;

public class BorrowState
{
    private readonly BooksService _booksService;
    public EventHandler<BookModel> BookModelUpdated { get; set; }
    public bool ShowingBookConfigurationDialog { get; private set; }
    public BookModel? CurrentBook { get; private set; }

    public string ErrorMessage { get; set; }
    
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

        BorrowModel borrowModel = null;
        try
        {
            borrowModel = await _booksService.BorrowBook(CurrentBook.Id, readersCardId);
        }
        catch (ClientException ce)
        {
            //TODO: call dialog to show message.
        }
        
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

        var returnedBook = -1;
        try
        {
            returnedBook = await _booksService.ReturnBook(bookId);
        }
        catch (ClientException ex)
        {
            //TODO: call dialog to show message.
        }
        
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