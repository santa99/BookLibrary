using Contracts.Models;
using View.Services;

namespace View.Data;

public interface IBorrowReturnViewService : IListViewService<BookModel>, ISelectable<int>
{
    /// <summary>
    /// Borrow state hold the state of the borrowed lend books.
    /// </summary>
    public BorrowState BorrowState { get; }

    /// <summary>
    /// Event handler fired when an item configured has been changed.
    /// </summary>
    public event EventHandler ItemChanged;
    
    /// <summary>
    /// Notify service that book needs to be returned.
    /// </summary>
    /// <returns></returns>
    Task NotifyBookReturned();
    
    /// <summary>
    /// Notify service that book need to be borrowed.
    /// </summary>
    /// <returns></returns>
    Task NotifyBookBorrowed();
}