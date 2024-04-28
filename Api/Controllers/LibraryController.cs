using Api.Filters;
using Attendance;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// [Authorize]
// [ServiceFilter(typeof(CustomAuthorizeFilter))]
public class LibraryController : Controller
{
    private readonly IBookLibraryRepository _bookLibraryRepository;
    private readonly IBookCommand _bookCommand;

    public LibraryController(IBookLibraryRepository bookLibraryRepository,
        IBookCommand bookCommand)
    {
        _bookLibraryRepository = bookLibraryRepository;
        _bookCommand = bookCommand;
    }
    
    [Route("/api/book/select/{bookState}")]
    public async Task<IActionResult> Index(int? bookState, CancellationToken cancellationToken)
    {
        bookState ??= (int)BookState.All;
        
        var bookModels = _bookLibraryRepository.ListBooks(bookState.Value);

        // ViewData["bookModel"] = bookModels;
        // return View(bookModels);
        return Ok(bookModels);
    }

    [Route("/api/book/remove/{bookId}")]
    public async Task<IActionResult> RemoveBook(int bookId, CancellationToken cancellationToken)
    {
        _bookLibraryRepository.RemoveBook(bookId);
        
        // var bookModels = _bookLibraryRepository.ListBooks((int)BookState.All);
        
        // return Ok(bookModels);
        return RedirectToPage("Index");
        // return RedirectToAction("Index");
        // ViewData["bookModel"] = bookModels;
        // return View(bookModels);
    }

    [Route("/api/book/edit/{bookId}")]
    public async Task<IActionResult> UpdateBook(int bookId,[FromQuery] string? title, [FromQuery] string? author, CancellationToken cancellationToken)
    {
        _bookLibraryRepository.UpdateBook(bookId, title, author);
        
        var bookModels = _bookLibraryRepository.ListBooks((int)BookState.All);
        
        return Ok(bookModels);
    }

    [Route("/api/book/add/")]
    public async Task<IActionResult> InsertBook([FromQuery] string title, [FromQuery] string author, CancellationToken cancellationToken)
    {
        _bookLibraryRepository.AddBook( title, author);
        
        var bookModels = _bookLibraryRepository.ListBooks((int)BookState.All);
        
        return Ok(bookModels);
    }

    /// <summary>
    /// Make a book as borrowed.
    /// </summary>
    /// <param name="bookId">Unique identifier of the book.</param>
    /// <param name="readersCardId">Unique id identifying the one who borrowed the book.</param>
    /// <param name="borrowed">Date when the book was borrowed from library.</param>
    [Route("/api/book/borrow/{bookId}/{readersCardId}")]
    public async Task<IActionResult> BorrowBook(int bookId, int readersCardId, DateTimeOffset borrowed)
    {
        _bookCommand.BorrowBook(bookId, readersCardId, borrowed);

        return Ok(bookId);
    }

    /// <summary>
    /// Return book back to the library.
    /// </summary>
    /// <param name="bookId">Unique identifier of the book.</param>
    [Route("/api/book/return/{bookId}")]
    public async Task<IActionResult> ReturnBook(int bookId)
    {
        _bookCommand.ReturnBook(bookId);

        return Ok(bookId);
    }
}