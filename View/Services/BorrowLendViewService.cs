using Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using View.Data;
using View.Exceptions;

namespace View.Services;

public class BorrowLendViewService : BookViewService, IBorrowLendViewService
{
    private readonly ReadersInfoService _readersInfoService;
    public event EventHandler<int>? ItemSelected;
    public event EventHandler? ItemChanged;

    /// <summary>
    /// Borrow state.
    /// </summary>
    public BorrowState BorrowState { get; }

    public BorrowLendViewService(BooksService booksService, ReadersInfoService readersInfoService) : base(booksService)
    {
        BorrowState = new BorrowState();
        _readersInfoService = readersInfoService;
    }
    
    public async Task NotifyItemSelected(object? sender, int bookId)
    {
        var bookResult = await BooksService.GetBook(bookId);
        if (bookResult is OkObjectResult { Value: BookModel bookModel })
        {
            if (bookModel.Borrowed == null)
            {
                var readersInfos = await _readersInfoService.GetAllReaders();
                var selected = readersInfos?.FirstOrDefault();
                BorrowState.ConfigureReaders(readersInfos, selected);
            }
            
            BorrowState.ShowConfigureDialog(bookModel);
            ItemSelected?.Invoke(sender, bookModel.Id);
        }
    }

    public async Task NotifyBookBorrowed()
    {
        var currentBook = BorrowState.CurrentBook;
        var reader = BorrowState.SelectedReader;
        
        try
        {
            if (currentBook == null || reader == null)
            {
                throw new SpecifiedException("Book can't be borrowed.");
            }
            
            await BooksService.BorrowBook(currentBook.Id, reader.ReaderCardId);
        }
        catch (ClientException exception)
        {
            //TODO: call dialog to show message.
        }
        
        BorrowState.CancelConfigureDialog();
        ItemChanged?.Invoke(this, null!);
        NotifyPageChanged(this, PaginationState.Page);
    }

    public async Task NotifyBookReturned()
    {
        var currentBook = BorrowState.CurrentBook;
        try
        {
            if (currentBook == null)
            {
                throw new SpecifiedException("Book can't be returned.");
            }

            await BooksService.ReturnBook(currentBook.Id);
        }
        catch (ClientException exception)
        {
            //TODO: call dialog to show message.
        }
        
        BorrowState.CancelConfigureDialog();
        ItemChanged?.Invoke(this, null);
        NotifyPageChanged(this, PaginationState.Page);
    }
    
}