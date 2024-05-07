using Contracts.Models;
using Microsoft.AspNetCore.Components;

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
        CurrentBook = new BookModel()
        {
            Author = bookModel.Author,
            Borrowed = bookModel.Borrowed,
            Id = bookModel.Id,
            Name = bookModel.Name
            
        };
        
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

    public void ReturnBook()
    {
        CurrentBook = null;
        ShowingConfigureDialog = false;
    }
}