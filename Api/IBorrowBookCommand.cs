using Contracts.Models;

namespace Api;

/// <summary>
/// Command contract providing two basic operations over the book a borrow and return.
/// </summary>
public interface IBorrowBookCommand
{
    /// <summary>
    /// Borrow book with the given <paramref name="bookId"/> to the provided <paramref name="readersCardId"/>.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="readersCardId">Readers card id.</param>
    /// <param name="borrowed">Borrowed time.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <exception cref="InvalidOperationException">When the book with <paramref name="bookId"/> hasn't been found.</exception>
    /// <exception cref="InvalidOperationException">When the provided <paramref name="readersCardId"/> hasn't been found.</exception>
    public Task<BorrowModel> BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed, CancellationToken cancellationToken);

    /// <summary>
    /// Returns book back to the library.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Returned book id.</returns>
    public Task<int> ReturnBook(int bookId, CancellationToken cancellationToken);
}