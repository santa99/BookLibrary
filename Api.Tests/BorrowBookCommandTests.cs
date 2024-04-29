using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Contracts;
using Contracts.Models;
using NSubstitute;
using Xunit;

namespace Api.Tests;

public class BorrowBookCommandTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoNSubstituteCustomization
        { ConfigureMembers = true });

    private readonly IBorrowBookCommand _sut;
    private readonly IBookLibraryRepository _bookLibraryRepository;

    public BorrowBookCommandTests()
    {
        _bookLibraryRepository = _fixture.Create<IBookLibraryRepository>();
        _sut = new BorrowBookCommand(_bookLibraryRepository);
    }

    [Fact]
    public async Task BorrowBook_Ok()
    {
        // Arrange
        var bookId = _fixture.Create<int>();
        var readersCardId = _fixture.Create<int>();
        var dateTimeOffset = _fixture.Create<DateTimeOffset>();
        var bookModel = _fixture.Create<BookModel>();
        bookModel.Borrowed = null;
        _bookLibraryRepository.GetBook(bookId, Arg.Any<CancellationToken>())
            .Returns(bookModel);

        // Act
        var borrowBook = await _sut.BorrowBook(bookId, readersCardId, dateTimeOffset, CancellationToken.None);

        // Assert
        Assert.NotNull(borrowBook);
        await _bookLibraryRepository.Received(1).BorrowBook(bookId, readersCardId, Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }
}