using Contracts;

namespace Api;

public class BookCommand : IBookCommand
{
    private readonly IBookLibraryRepository _bookLibraryRepository;
    private readonly IReadersInfoRepository _readersInfoRepository;

    public BookCommand(
        IBookLibraryRepository bookLibraryRepository,
        IReadersInfoRepository readersInfoRepository)
    {
        _bookLibraryRepository = bookLibraryRepository;
        _readersInfoRepository = readersInfoRepository;
    }


    public void BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed)
    {
        var book = _bookLibraryRepository.GetBook(bookId);

        if (book == null)
        {
            throw new InvalidOperationException($"Requested bookId: {bookId} does not exist");
        }
        
        var info = _readersInfoRepository.GetReadersInfo(readersCardId);
        if (info == null)
        {
            throw new InvalidOperationException($"Requested readers card info: '{readersCardId}' does not exist.");
        }

        if (book.Borrowed != null)
        {
            throw new InvalidOperationException($"Requested book has been already borrowed.");
        }
        
        _bookLibraryRepository.BorrowBook(bookId, info.Name, info.LastName, borrowed);
    }

    public void ReturnBook(int bookId)
    {
        var book = _bookLibraryRepository.GetBook(bookId);
        
        if (book == null)
        {
            throw new InvalidOperationException($"Requested bookId: {bookId} does not exist");
        }
        
        _bookLibraryRepository.ReturnBook(bookId);
    }
}