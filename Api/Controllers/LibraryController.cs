using System.Net;
using System.Net.Mime;
using Api.Filters;
using Api.Mappers;
using Api.Models;
using Contracts;
using Contracts.Enums;
using Contracts.Models;
using Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <inheritdoc />
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[ProducesResponseType(typeof(List<ErrorCodeModel>), StatusCodes.Status401Unauthorized)]
[Produces( MediaTypeNames.Application.Json )]
public class LibraryController : Controller
{
    private readonly IBookLibraryRepository _bookLibraryRepository;
    private readonly IBorrowBookCommand _borrowBookCommand;
    private readonly BookStateMapper _bookStateMapper;

    /// <inheritdoc />
    public LibraryController(IBookLibraryRepository bookLibraryRepository,
        IBorrowBookCommand borrowBookCommand,
        BookStateMapper bookStateMapper)
    {
        _bookLibraryRepository = bookLibraryRepository;
        _borrowBookCommand = borrowBookCommand;
        _bookStateMapper = bookStateMapper;
    }

    /// <summary>
    /// Selects all books from the library.
    /// </summary>
    /// <param name="bookState"><see cref="BookState"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <param name="start">Start count.</param>
    /// <param name="count">Number of element to read.</param>
    /// <returns>List of <see cref="BookModel"/></returns>
    [HttpGet("/api/book/select/{bookState}/{start}/{count}")]
    [ProducesResponseType(typeof(List<BookModel>), StatusCodes.Status200OK)]
    public async Task<List<BookModel>> Index([FromRoute] int? bookState, CancellationToken cancellationToken, [FromRoute] int start = 0, [FromRoute] int count = -1)
    {
        bookState ??= (int)BookState.All;

        var bookModels =
            await _bookLibraryRepository.ListBooks(_bookStateMapper.Map(bookState.Value), cancellationToken, count, start);

        return bookModels;
    }

    /// <summary>
    /// Get book based on the book id from the library.
    /// </summary>
    /// <param name="bookId">Unique book id.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="BookModel"/> when sucessed.</returns>
    [HttpGet("/api/book/get/{bookId}")]
    [ProducesResponseType(typeof(BookModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BookModel), StatusCodes.Status404NotFound)]
    public async Task<BookModel?> GetBook(int bookId, CancellationToken cancellationToken)
    {
        var bookModel = await _bookLibraryRepository.GetBook(bookId, cancellationToken);
        if (bookModel == null)
        {
            NotFound();
        }
        return bookModel;
    }

    /// <summary>
    /// Removes the book from the library.
    /// </summary>
    /// <param name="bookId">Unique book id.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [HttpDelete("/api/book/remove/{bookId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveBook(int bookId, CancellationToken cancellationToken)
    {
        await _bookLibraryRepository.RemoveBook(bookId, cancellationToken);
        
        return NoContent();
    }

    /// <summary>
    /// Update existing book.
    /// </summary>
    /// <param name="updateBookReqModel"><see cref="UpdateBookReqModel"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns></returns>
    [HttpPatch("/api/book/edit/{BookId}")]
    [ServiceFilter(typeof(RequestModelValidationFilter))]
    [ProducesResponseType(typeof(BookModel),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorCodeModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorCodeModel), StatusCodes.Status404NotFound)]
    public async Task<BookModel?> UpdateBook(UpdateBookReqModel updateBookReqModel,
        CancellationToken cancellationToken)
    {
        await _bookLibraryRepository.UpdateBookDetails(updateBookReqModel.BookId, updateBookReqModel.Title,
            updateBookReqModel.Author, cancellationToken);
        var bookUpdated = await _bookLibraryRepository.GetBook(updateBookReqModel.BookId, cancellationToken);

        return bookUpdated;
    }

    /// <summary>
    /// Insert a new book into the library.
    /// </summary>
    /// <param name="createBookReqModel"><see cref="CreateBookReqModel"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Completed action with valid data or invalid <see cref="ErrorCodeModel"/>.</returns>
    [HttpGet("/api/book/add/")]
    [ServiceFilter(typeof(RequestModelValidationFilter))]
    [ProducesResponseType(typeof(BookModel), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorCodeModel), StatusCodes.Status400BadRequest)]
    public async Task<BookModel?> InsertBook([FromRoute] CreateBookReqModel createBookReqModel,
        CancellationToken cancellationToken)
    {
        var bookId =
            await _bookLibraryRepository.AddNewBook(createBookReqModel.Title, createBookReqModel.Author,
                cancellationToken);
        var bookInserted = await _bookLibraryRepository.GetBook(bookId, cancellationToken);

        if (bookInserted != null)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
        }
        
        return bookInserted;
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
    [ProducesResponseType(typeof(ErrorCodeModel), StatusCodes.Status404NotFound)]
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
    [HttpGet("/api/book/return/{bookId}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReturnBook(int bookId, CancellationToken cancellationToken)
    {
        var bookIdReturned = await _borrowBookCommand.ReturnBook(bookId, cancellationToken);

        return Ok(bookIdReturned);
    }
}