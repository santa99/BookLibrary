namespace Contracts;

/// <summary>
/// Contract the API can access data from layer below.
/// </summary>
public interface IBookLibraryRepository
{
    /// <summary>
    /// Removes the book from the repository.
    /// </summary>
    /// <param name="bookId">Unique identifier of the book.</param>
    public void RemoveBook(int bookId);
    
    /// <summary>
    /// Adds a new book into the library of the books.
    /// New book is implicitly not borrowed.
    /// </summary>
    /// <param name="name">Name of the title.</param>
    /// <param name="author">Author of the book.</param>
    /// <returns>Unique id of the book</returns>
    public int AddNewBook(string name, string author);

    /// <summary>
    /// Updates existing book in library with the new values.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="name">Name of the title.</param>
    /// <param name="author">Author of the book.</param>
    public void UpdateBookDetails(int bookId, string? name, string? author);

    /// <summary>
    /// Borrow the book with provided id.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <param name="readersCardId">Readers card.</param>
    /// <param name="from">When was the book borrowed.</param>
    public void BorrowBook(int bookId, int readersCardId, DateTimeOffset from);

    /// <summary>
    /// Returns the book back to the library.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    public void ReturnBook(int bookId);
    
    /// <summary>
    /// Retrieves book from the library based on the book id.
    /// </summary>
    /// <param name="bookId">Unique id of the book.</param>
    /// <returns>Book or null if the provided is not found.</returns>
    public BookModel? GetBook(int bookId);

    /// <summary>
    /// Provide a list of all books in the library.
    /// </summary>
    /// <param name="bookStateId">State of the book.</param>
    /// <param name="count">Restrict number of books from library.</param>
    /// <param name="start">Relative start to retrieve books from. Perfect for pagination purpose.</param>
    /// <returns>A list of all books.</returns>
    public List<BookModel> ListBooks(int bookStateId, int count = -1, int start = 0);
}