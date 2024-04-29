namespace Contracts;

/// <summary>
/// Class <see cref="IBookLibraryDao"/> represents data access object to book library.
/// </summary>
public interface IBookLibraryDao
{
    /// <summary>
    /// Adds a new book into the storage.
    /// </summary>
    /// <param name="bookModel">Book model.</param>
    /// <returns>Id of the book.</returns>
    int Create(BookModel bookModel);

    /// <summary>
    /// Tries to get book by the <paramref name="bookId"/>.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <returns><see cref="BookModel"/> or null if the provided id is not found.</returns>
    BookModel? Read(int bookId);

    /// <summary>
    ///  Updates the book in storage.
    /// </summary>
    /// <param name="bookModel">Updated book model.</param>
    void Update(BookModel bookModel);

    /// <summary>
    /// Deletes the book from storage.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    void Delete(int bookId);

    /// <summary>
    /// Provides the books list.
    /// </summary>
    /// <returns>List of books.</returns>
    List<BookModel> GetBooks();
}