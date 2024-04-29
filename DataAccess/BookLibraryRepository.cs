using Contracts;

namespace DataAccess;

/// <summary>
/// Class <see cref="BookLibraryRepository"/> is the repository for the book library.
/// </summary>
public class BookLibraryRepository : IBookLibraryRepository
{
    private readonly IBookLibraryDao _bookLibraryDao;

    public BookLibraryRepository(IBookLibraryDao bookLibraryDao)
    {
        _bookLibraryDao = bookLibraryDao;
    }

    public void RemoveBook(int bookId)
    {
        _bookLibraryDao.Delete(bookId);
    }

    public int AddBook(string name, string author)
    {
        return _bookLibraryDao.Create(name, author);
    }

    public BookModel? GetBook(int bookId)
    {
        return _bookLibraryDao.Read(bookId);
    }

    public void UpdateBook(int bookId, string? name, string? author)
    {
        _bookLibraryDao.Update(bookId, name, author);
    }

    public void BorrowBook(int bookId, string firstName, string lastName, DateTimeOffset from)
    {
        _bookLibraryDao.BorrowBook(bookId, firstName, lastName, from);
    }

    public void ReturnBook(int bookId)
    {
        _bookLibraryDao.ReturnBook(bookId);
    }

    public List<BookModel> ListBooks(int bookStateId, int count = -1, int start = 0)
    {
        var bookModels = _bookLibraryDao.GetBooks();

        return bookStateId switch
        {
            -1 => bookModels,
            0 => bookModels.Where(model => model.Borrowed == null).ToList(),
            1 => bookModels.Where(model => model.Borrowed != null).ToList(),
            _ => bookModels
        };
    }
}