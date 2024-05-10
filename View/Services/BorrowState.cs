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

    public async Task<BookModel> BorrowBook(int readersCardId)
    {
        var borrowModel = await _booksService.BorrowBook(CurrentBook.Id, readersCardId);
        var borrowedBook = CurrentBook;
        CurrentBook = null;
        ShowingConfigureDialog = false;
        return new BookModel()
        {
            Id = borrowedBook.Id,
            Name = borrowedBook.Name,
            Author = borrowedBook.Author,
            Borrowed = borrowModel
        };
    }

    public async Task<BookModel> ReturnBook(int bookId)
    {
        await _booksService.ReturnBook(bookId);
        var returnedBook = CurrentBook;
        CurrentBook = null;
        ShowingConfigureDialog = false;
        return new BookModel()
        {
            Id = returnedBook.Id,
            Name = returnedBook.Name,
            Author = returnedBook.Author,
            Borrowed = null
        };
    }
}