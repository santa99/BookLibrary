using Contracts;
using Contracts.Exceptions;

namespace DataAccess;

/// <summary>
///     Class <see cref="BookLibraryRepository" /> is the repository for the book library.
/// </summary>
public class BookLibraryRepository : IBookLibraryRepository
{
    private readonly IBookLibraryDao _bookLibraryDao;
    private readonly IReadersInfoDao _readersInfoDao;

    public BookLibraryRepository(IBookLibraryDao bookLibraryDao, IReadersInfoDao readersInfoDao)
    {
        _bookLibraryDao = bookLibraryDao;
        _readersInfoDao = readersInfoDao;
    }

    public Task<int> AddNewBook(string name, string author, CancellationToken cancellationToken)
    {
        var bookId = _bookLibraryDao.Create(new BookModel
        {
            Name = name,
            Author = author
        });

        return Task.FromResult(bookId);
    }

    public Task RemoveBook(int bookId, CancellationToken cancellationToken)
    {
        _bookLibraryDao.Delete(bookId);

        return Task.CompletedTask;
    }

    public Task<BookModel?> GetBook(int bookId)
    {
        return Task.FromResult(_bookLibraryDao.Read(bookId));
    }

    public Task UpdateBookDetails(int bookId, string? name, string? author, CancellationToken cancellationToken)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null)
        {
            throw new BookLibraryException($"Requested bookId: {bookId} does not exist.", ErrorCode.BookNotFound);
        }

        _bookLibraryDao.Update(new BookModel
        {
            Id = bookId,
            Name = string.IsNullOrWhiteSpace(name) ? bookModel.Name : name,
            Author = string.IsNullOrWhiteSpace(author) ? bookModel.Author : author
        });
        
        return Task.CompletedTask;
    }

    public Task<BorrowModel> BorrowBook(int bookId, int readersCardId, DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null)
        {
            throw new BookLibraryException($"Requested bookId: {bookId} does not exist.", ErrorCode.BookNotFound);
        }

        if (bookModel.Borrowed != null)
        {
            throw new BookLibraryException(
                $"Requested book '{bookId}':'{bookModel.Name}' has already been borrowed.", ErrorCode.BorrowedBook);
        }

        var readerInfo = _readersInfoDao.Read(readersCardId);
        if (readerInfo == null)
        {
            throw new BookLibraryException($"Requested readers card info: '{readersCardId}' does not exist.", ErrorCode.ReadersCardNotFound);
        }

        bookModel.Borrowed = new BorrowModel
        {
            From = from,
            FirstName = readerInfo.FirstName,
            LastName = readerInfo.LastName
        };

        _bookLibraryDao.Update(bookModel);

        return Task.FromResult(bookModel.Borrowed);
    }

    public Task<int> ReturnBook(int bookId, CancellationToken cancellationToken)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null)
        {
            throw new BookLibraryException($"Requested bookId: {bookId} does not exist.", ErrorCode.BookNotFound);
        }

        if (bookModel.Borrowed == null)
        {
            return Task.FromResult(-1);
        }

        bookModel.Borrowed = null;

        _bookLibraryDao.Update(bookModel);

        return Task.FromResult(bookModel.Id);
    }

    public Task<List<BookModel>> ListBooks(BookState bookState, CancellationToken cancellationToken, int count = -1, int start = 0)
    {
        var bookModels = _bookLibraryDao.GetBooks();

        count = count == -1 ? bookModels.Count : count;
        var rangeWindow = start + count;
        var range = bookModels.Where((_, i) => i >= start && i < rangeWindow).ToList();

        return Task.FromResult(bookState switch
        {
            BookState.All => range,
            BookState.Free => range.Where(model => model.Borrowed == null).ToList(),
            BookState.Borrowed => range.Where(model => model.Borrowed != null).ToList(),
            _ => bookModels
        });
    }
}