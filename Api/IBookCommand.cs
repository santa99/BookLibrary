namespace Api;

public interface IBookCommand
{
    /// <summary>
    /// Borrow book with the given <paramref name="bookId"/> to <paramref name="readersCardId"/>.
    /// </summary>
    /// <param name="bookId"></param>
    /// <param name="readersCardId"></param>
    /// <param name="borrowed"></param>
    public void BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed);

    /// <summary>
    /// Returns book back to the library.
    /// </summary>
    /// <param name="bookId"></param>
    public void ReturnBook(int bookId);
}