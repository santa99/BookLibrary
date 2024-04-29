using Api.Filters;
using Api.Mappers;
using Api.Models;
using Api.Models.Responses;
using Contracts;
using Contracts.Enums;
using Contracts.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

// [Authorize]
[ServiceFilter(typeof(CustomAuthorizeFilter))]
[ProducesResponseType(typeof(List<ErrorCodeModel>), StatusCodes.Status401Unauthorized)]
public class LibraryController : Controller
{
    private readonly IBookLibraryRepository _bookLibraryRepository;
    private readonly IBorrowBookCommand _borrowBookCommand;
    private readonly BookStateMapper _bookStateMapper;

    public LibraryController(IBookLibraryRepository bookLibraryRepository,
        IBorrowBookCommand borrowBookCommand,
        BookStateMapper bookStateMapper)
    {
        _bookLibraryRepository = bookLibraryRepository;
        _borrowBookCommand = borrowBookCommand;
        _bookStateMapper = bookStateMapper;
    }

    [Route("/api/book/select/{bookState}")]
    [ProducesResponseType(typeof(List<BookModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Index(int? bookState, CancellationToken cancellationToken)
    {
        bookState ??= (int)BookState.All;

        var bookModels =
            await _bookLibraryRepository.ListBooks(_bookStateMapper.Map(bookState.Value), cancellationToken);

        return Ok(bookModels);
    }

    [HttpGet("/api/book/get/{bookId}")]
    [ProducesResponseType(typeof(BookModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBook(int bookId, CancellationToken cancellationToken)
    {
        var bookModel = await _bookLibraryRepository.GetBook(bookId, cancellationToken);

        return Ok(bookModel);
    }

    [Route("/api/book/remove/{bookId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveBook(int bookId, CancellationToken cancellationToken)
    {
        await _bookLibraryRepository.RemoveBook(bookId, cancellationToken);

        return Ok();
    }

    [HttpGet("/api/book/edit/{bookId}")]
    [ServiceFilter(typeof(RequestModelValidationFilter))]
    [ProducesResponseType(typeof(BookModel),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorCodeModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBook(UpdateBookReqModel updateBookReqModel,
        CancellationToken cancellationToken)
    {
        await _bookLibraryRepository.UpdateBookDetails(updateBookReqModel.BookId, updateBookReqModel.Title,
            updateBookReqModel.Author, cancellationToken);
        var bookUpdated = await _bookLibraryRepository.GetBook(updateBookReqModel.BookId, cancellationToken);

        return Ok(bookUpdated);
    }

    [HttpGet("/api/book/add/")]
    [ServiceFilter(typeof(RequestModelValidationFilter))]
    [ProducesResponseType(typeof(BookModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorCodeModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InsertBook([FromRoute] CreateBookReqModel createBookReqModel,
        CancellationToken cancellationToken)
    {
        var bookId =
            await _bookLibraryRepository.AddNewBook(createBookReqModel.Title, createBookReqModel.Author,
                cancellationToken);
        var bookInserted = await _bookLibraryRepository.GetBook(bookId, cancellationToken);

        return Ok(bookInserted);
    }

    /// <summary>
    /// Borrow a book from the library.
    /// </summary>
    /// <param name="borrowReqModel"><see cref="CreateBorrowReqModel"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    [HttpGet("/api/book/borrow/{bookId}/{readersCardId}")]
    [ServiceFilter(typeof(RequestModelValidationFilter))]
    [ProducesResponseType(typeof(BorrowModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorCodeModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BorrowBook([FromRoute] CreateBorrowReqModel borrowReqModel,
        CancellationToken cancellationToken)
    {
        var borrowModel = await _borrowBookCommand.BorrowBook(borrowReqModel.BookId, borrowReqModel.ReadersCardId,
            borrowReqModel.From, cancellationToken);

        return Ok(borrowModel);
    }

    /// <summary>
    /// Return the book back to the library.
    /// </summary>
    /// <param name="bookId">Unique identifier of the book.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    [Route("/api/book/return/{bookId}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReturnBook(int bookId, CancellationToken cancellationToken)
    {
        var bookIdReturned = await _borrowBookCommand.ReturnBook(bookId, cancellationToken);

        return Ok(bookIdReturned);
    }
}