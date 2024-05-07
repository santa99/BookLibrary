using Contracts.Models;

namespace View.Services;

public class BorrowState
{
    /// <summary>
    /// Dialog is being displayed when true.
    /// </summary>
    public bool ShowingConfigureDialog { get; private set; }
    
    /// <summary>
    /// Currently configured book.
    /// </summary>
    public BookModel CurrentBook { get; private set; }

    /// <summary>
    /// Show configuration dialog over the book.
    /// </summary>
    /// <param name="bookModel"></param>
    public void ShowConfigureDialog(BookModel bookModel)
    {
        CurrentBook = bookModel;
        
        ShowingConfigureDialog = true;
    }

    /// <summary>
    /// Cancel configure dialog
    /// </summary>
    public void CancelConfigureDialog()
    {
        CurrentBook = null;

        ShowingConfigureDialog = false;
    }

    /// <summary>
    /// Borrow book.
    /// </summary>
    public void BorrowBook()
    {
        
        CurrentBook = null;
        ShowingConfigureDialog = false;
    }
}