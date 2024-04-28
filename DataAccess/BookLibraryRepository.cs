using Contracts;

namespace DataAccess;

/// <summary>
/// Class <see cref="BookLibraryRepository"/> is the repository for the book library.
/// </summary>
public class BookLibraryRepository : IBookLibraryRepository
{
    private readonly ILibraryDb _libraryDb;

    public BookLibraryRepository(ILibraryDb libraryDb)
    {
        _libraryDb = libraryDb;
    }

    public void RemoveBook(int bookId)
    {
        _libraryDb.RemoveBook(bookId);
    }

    public int AddBook(string name, string author)
    {
        return _libraryDb.InsertBook(name, author);
    }

    public BookModel? GetBook(int bookId)
    {
        return _libraryDb.GetBook(bookId);
    }

    public void UpdateBook(int bookId, string name, string author)
    {
        _libraryDb.UpdateBook(bookId, name, author);
    }

    public void BorrowBook(int bookId, string firstName, string lastName, DateTimeOffset from)
    {
        _libraryDb.BorrowBook(bookId, firstName, lastName, from);
    }

    public void ReturnBook(int bookId)
    {
        _libraryDb.ReturnBook(bookId);
    }

    public List<BookModel> ListBooks(int bookStateId, int count = -1, int start = 0)
    {
        var bookModels = _libraryDb.GetBooks();

        return bookStateId switch
        {
            -1 => bookModels,
            0 => bookModels.Where(model => model.BorrowedBy == null).ToList(),
            1 => bookModels.Where(model => model.BorrowedBy != null).ToList(),
            _ => bookModels
        };
    }
}