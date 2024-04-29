namespace Contracts;

/// <summary>
/// Class <see cref="IBookLibraryDao"/> represents data access object to book library.
/// </summary>
public interface IBookLibraryDao
{
    /// <summary>
    /// Adds a new book into the storage.
    /// </summary>
    /// <param name="name">Name of the title.</param>
    /// <param name="author">Author of the book.</param>
    /// <returns>Id of the book.</returns>
    int Create(string name, string author);
    
    /// <summary>
    /// Tries to get book by the <paramref name="bookId"/>.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <returns><see cref="BookModel"/> or null if the provided id is not found.</returns>
    BookModel? Read(int bookId);
    
    /// <summary>
    /// Updates the book in storage.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="name">Name of the book.</param>
    /// <param name="author">Author of the book.</param>
    void Update(int bookId, string? name, string? author);
    
    /// <summary>
    /// Deletes the book from storage.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    void Delete(int bookId);
    
    List<BookModel> GetBooks();
    void BorrowBook(int bookId, string firstName, string lastName, DateTimeOffset from);
    void ReturnBook(int bookId);
}