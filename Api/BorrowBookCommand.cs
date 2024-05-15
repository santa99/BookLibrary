using Contracts;
using Contracts.Models;

namespace Api;

/// <inheritdoc />
public class BorrowBookCommand : IBorrowBookCommand
{
    private readonly IBookLibraryRepository _bookLibraryRepository;

    public BorrowBookCommand(
        IBookLibraryRepository bookLibraryRepository)
    {
        _bookLibraryRepository = bookLibraryRepository;
    }

    /// <inheritdoc />
    public Task<BorrowModel> BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed,
        CancellationToken cancellationToken)
    {
        return _bookLibraryRepository.BorrowBook(bookId, readersCardId, borrowed, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> ReturnBook(int bookId, CancellationToken cancellationToken)
    {
        return _bookLibraryRepository.ReturnBook(bookId, cancellationToken);
    }
}