using Contracts;

namespace Api;

public class BorrowBookCommand : IBorrowBookCommand
{
    private readonly IBookLibraryRepository _bookLibraryRepository;

    public BorrowBookCommand(
        IBookLibraryRepository bookLibraryRepository)
    {
        _bookLibraryRepository = bookLibraryRepository;
    }


    public void BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed)
    {
        var book = _bookLibraryRepository.GetBook(bookId);

        ValidateBook(bookId, book);
        
        _bookLibraryRepository.BorrowBook(bookId, readersCardId, borrowed);
    }

    public void ReturnBook(int bookId)
    {
        var book = _bookLibraryRepository.GetBook(bookId);
        
        ValidateBook(bookId, book);
        
        _bookLibraryRepository.ReturnBook(bookId);
    }

    private static void ValidateBook(int bookId, BookModel? book)
    {
        if (book == null)
        {
            throw new InvalidOperationException($"Requested bookId: {bookId} does not exist");
        }
    }
}