using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class LibraryController : ControllerBase
{
    private readonly IBookLibraryRepository _bookLibraryRepository;

    public LibraryController(IBookLibraryRepository bookLibraryRepository)
    {
        _bookLibraryRepository = bookLibraryRepository;
    }

    [Route("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var bookModels = _bookLibraryRepository.ListBooks(0);
        
        return Ok(bookModels);
    }

    [Route("/api/book/remove/{bookId}")]
    public async Task<IActionResult> RemoveBook(int bookId, CancellationToken cancellationToken)
    {
        _bookLibraryRepository.RemoveBook(bookId);
        
        var bookModels = _bookLibraryRepository.ListBooks(0);
        
        return Ok(bookModels);
    }

    [Route("/api/book/edit/{bookId}")]
    public async Task<IActionResult> UpdateBook(int bookId,[FromQuery] string title, [FromQuery] string author, CancellationToken cancellationToken)
    {
        _bookLibraryRepository.UpdateBook(bookId, title, author);
        
        var bookModels = _bookLibraryRepository.ListBooks(0);
        
        return Ok(bookModels);
    }

    [Route("/api/book/add/")]
    public async Task<IActionResult> InsertBook([FromQuery] string title, [FromQuery] string author, CancellationToken cancellationToken)
    {
        _bookLibraryRepository.AddBook( title, author);
        
        var bookModels = _bookLibraryRepository.ListBooks(0);
        
        return Ok(bookModels);
    }

    /// <summary>
    /// Make a book as borrowed.
    /// </summary>
    /// <param name="bookId">Unique identifier of the book.</param>
    /// <param name="readersCardId">Unique id identifying the one who borrowed the book.</param>
    /// <param name="borrowed">Date when the book was borrowed from library.</param>
    public void BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed)
    {
        
    }

    /// <summary>
    /// Return book back to the library.
    /// </summary>
    /// <param name="bookId">Unique identifier of the book.</param>
    /// <param name="returned">Date when the book was returned to library.</param>
    public void ReturnBook(int bookId, DateTimeOffset returned)
    {
        
    }
}