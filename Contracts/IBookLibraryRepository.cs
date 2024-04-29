namespace Contracts;

/// <summary>
/// Contract so that the API can access data from book library repository.
/// </summary>
public interface IBookLibraryRepository
{
    /// <summary>
    /// Removes the book from the repository.
    /// </summary>
    /// <param name="bookId">Unique identifier of the book.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public Task RemoveBook(int bookId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new book into the library of the books.
    /// New book is implicitly not borrowed.
    /// </summary>
    /// <param name="name">Name of the title.</param>
    /// <param name="author">Author of the book.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Unique id of the book</returns>
    public Task<int> AddNewBook(string name, string author, CancellationToken cancellationToken);

    /// <summary>
    /// Updates existing book in library with the new values.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="name">Name of the title.</param>
    /// <param name="author">Author of the book.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public Task UpdateBookDetails(int bookId, string? name, string? author, CancellationToken cancellationToken);

    /// <summary>
    /// Borrow the book with provided id.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="readersCardId">Readers card.</param>
    /// <param name="from">When was the book borrowed.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    public Task<BorrowModel> BorrowBook(int bookId, int readersCardId, DateTimeOffset from,
        CancellationToken cancellationToken);

    /// <summary>
    /// Returns the book back to the library.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Task returning book id return or -1 when not borrowed.</returns>
    public Task<int> ReturnBook(int bookId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Retrieves book from the library based on the book id.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <returns>Book or null if the provided is not found.</returns>
    public Task<BookModel?> GetBook(int bookId);

    /// <summary>
    /// Provide a list of all books in the library.
    /// </summary>
    /// <param name="bookStateId">State of the book.</param>
    /// <param name="count">Restrict number of books from library.</param>
    /// <param name="start">Relative start to retrieve books from. Perfect for pagination purpose.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>A list of all books.</returns>
    public Task<List<BookModel>> ListBooks(BookState bookStateId, CancellationToken cancellationToken, int count = -1, int start = 0);
}