using Contracts;

namespace DataAccess;

/// <summary>
///     Class <see cref="BookLibraryRepository" /> is the repository for the book library.
/// </summary>
public class BookLibraryRepository : IBookLibraryRepository
{
    private readonly IBookLibraryDao _bookLibraryDao;

    public BookLibraryRepository(IBookLibraryDao bookLibraryDao)
    {
        _bookLibraryDao = bookLibraryDao;
    }
    
    public int AddNewBook(string name, string author)
    {
        return _bookLibraryDao.Create(new BookModel
        {
            Name = name,
            Author = author
        });
    }

    public void RemoveBook(int bookId)
    {
        _bookLibraryDao.Delete(bookId);
    }
    
    public BookModel? GetBook(int bookId)
    {
        return _bookLibraryDao.Read(bookId);
    }

    public void UpdateBookDetails(int bookId, string? name, string? author)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null) return;

        _bookLibraryDao.Update(new BookModel
        {
            Id = bookId,
            Name = string.IsNullOrWhiteSpace(name) ? bookModel.Name : name,
            Author = string.IsNullOrWhiteSpace(author) ? bookModel.Author : author
        });
    }

    //Todo: set readers info and retrieve additional info from readersinfodao.
    public void BorrowBook(int bookId, string firstName, string lastName, DateTimeOffset from)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null) return;

        bookModel.Borrowed = new BorrowModel
        {
            From = from,
            FirstName = firstName,
            LastName = lastName
        };

        _bookLibraryDao.Update(bookModel);
    }

    public void ReturnBook(int bookId)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null) return;

        bookModel.Borrowed = null;

        _bookLibraryDao.Update(bookModel);
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