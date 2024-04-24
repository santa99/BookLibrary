using Contracts;

namespace DataAccess;

/// <summary>
/// Class <see cref="BookLibraryRepository"/> is the repository for the book library.
/// </summary>
public class BookLibraryRepository : IBookLibraryRepository
{
    private readonly ILibraryDb _libraryDb;

    public BookLibraryRepository(XmlDatabase libraryDb)
    {
        _libraryDb = libraryDb;
    }

    public void RemoveBook(int bookId)
    {
        _libraryDb.RemoveBook(bookId);
    }

    public void BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed)
    {
        var book = _libraryDb.GetBook(bookId);
        if (book == null)
        {
            return;
        }
        
        var info = _libraryDb.GetReadersInfo(readersCardId);
        if (info == null)
        {
            return;
        }
        
        book.BorrowedBy = new BorrowDto
        {
            From = borrowed,
            FirstName = info.Name,
            LastName = info.LastName
        };
    }

    public void ReturnBook(int bookId, DateTimeOffset returned)
    {
        var book = _libraryDb.GetBook(bookId);
        if (book != null)
        {
            book.BorrowedBy = null;
        }
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

    public List<BookModel> ListBooks(int bookStateId, int count = -1, int start = 0)
    {
        // _libraryDb.LoadLibrary(@"C:");
        
        return _libraryDb.GetBooks();
    }
}