using Contracts;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Models;

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

    public Task<BookModel?> GetBook(int bookId, CancellationToken cancellationToken)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null)
        {
            throw new BookNotFoundException(bookId);
        }
        return Task.FromResult(bookModel);
    }

    public Task UpdateBookDetails(int bookId, string? name, string? author, CancellationToken cancellationToken)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null)
        {
            throw new BookNotFoundException(bookId);
        }

        _bookLibraryDao.Update(new BookModel
        {
            Id = bookId,
            Name = string.IsNullOrWhiteSpace(name) ? bookModel.Name : name,
            Author = string.IsNullOrWhiteSpace(author) ? bookModel.Author : author,
            Borrowed = bookModel.Borrowed
        });
        
        return Task.CompletedTask;
    }

    public Task<BorrowModel> BorrowBook(int bookId, int readersCardId, DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var bookModel = _bookLibraryDao.Read(bookId);
        if (bookModel == null)
        {
            throw new BookNotFoundException(bookId);
        }

        if (bookModel.Borrowed != null)
        {
            throw new BookLibraryException(
                $"Requested book '{bookId}':'{bookModel.Name}' has already been borrowed.", ErrorCode.BorrowedBook);
        }

        var readerInfo = _readersInfoDao.Read(readersCardId);
        if (readerInfo == null)
        {
            throw new ReadersCardIdNotFoundException(readersCardId);
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
            throw new BookNotFoundException(bookId);
        }

        if (bookModel.Borrowed == null)
        {
            throw new BookLibraryException(
                $"Requested book '{bookId}':'{bookModel.Name}' hasn't been borrowed.", ErrorCode.BookNotBorrowed);
        }

        bookModel.Borrowed = null;

        _bookLibraryDao.Update(bookModel);

        return Task.FromResult(bookModel.Id);
    }

    public Task<List<BookModel>> ListBooks(BookState bookState, CancellationToken cancellationToken, int count = -1, int start = 0)
    {
        var bookModels = _bookLibraryDao.GetBooks();
        
        bookModels = bookState switch
        {
            BookState.All => bookModels,
            BookState.Free => bookModels.Where(model => model.Borrowed == null).ToList(),
            BookState.Borrowed => bookModels.Where(model => model.Borrowed != null).ToList(),
            _ => bookModels
        };
        
        count = count == -1 ? bookModels.Count : count;
        var rangeWindow = start + count;
        var range = bookModels.Where((_, i) => i >= start && i < rangeWindow).ToList();

        return Task.FromResult(range);
    }
}