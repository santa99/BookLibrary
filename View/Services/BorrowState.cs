using Contracts.Models;

namespace View.Services;

/// <summary>
/// Class <see cref="BorrowState"/> represent state holding currently selected book and potentially
/// available readers for a new borrow.
/// </summary>
public class BorrowState
{
    /// <summary>
    /// Flag telling the ui that configuration dialog should be opened or not.
    /// </summary>
    public bool ShowingBookConfigurationDialog { get; private set; }
    
    /// <summary>
    /// Currently selected book.
    /// </summary>
    public BookModel? CurrentBook { get; private set; }

    /// <summary>
    /// Currently selected reader.
    /// </summary>
    public ReadersInfo? SelectedReader { get; private set; }
    
    /// <summary>
    /// Readers list.
    /// </summary>
    public List<ReadersInfo> ReadersList { get; private set; } = new();
    
    /// <summary>
    /// Select reader.
    /// </summary>
    /// <param name="readersId">Readers Id.</param>
    public void SelectReader(int readersId)
    {
        SelectedReader = ReadersList.FirstOrDefault(info => info.ReaderCardId == readersId);
    }

    /// <summary>
    /// Configure readers.
    /// </summary>
    /// <param name="readersInfos">Readers info.</param>
    /// <param name="selected">Current reader.</param>
    internal void ConfigureReaders(List<ReadersInfo> readersInfos, ReadersInfo selected)
    {
        ReadersList = readersInfos;
        SelectedReader = selected;
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
        ShowingBookConfigurationDialog = true;
        
        CurrentBook = new BookModel
        {
            Author = bookModel.Author,
            Borrowed = bookModel.Borrowed,
            Id = bookModel.Id,
            Name = bookModel.Name
        };
    }

    /// <summary>
    /// Call this method if you want to close dialog.
    /// </summary>
    public void CancelConfigureDialog()
    {
        CurrentBook = null;
        ShowingBookConfigurationDialog = false;
    }

}