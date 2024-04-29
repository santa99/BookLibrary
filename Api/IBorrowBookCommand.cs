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
    /// <exception cref="InvalidOperationException">When the book with <paramref name="bookId"/> hasn't been found.</exception>
    /// <exception cref="InvalidOperationException">When the provided <paramref name="readersCardId"/> hasn't been found.</exception>
    public void BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed);

    /// <summary>
    /// Returns book back to the library.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    public void ReturnBook(int bookId);
}