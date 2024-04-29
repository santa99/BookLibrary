using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Contracts;
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
    public void BorrowBook_Ok()
    {
        // Arrange
        var bookId = _fixture.Create<int>();
        var readersCardId = _fixture.Create<int>();
        var dateTimeOffset = _fixture.Create<DateTimeOffset>();
        var bookModel = _fixture.Create<BookModel>();
        bookModel.Borrowed = null;
        _bookLibraryRepository.GetBook(bookId)
            .Returns(bookModel);

        // Act
        _sut.BorrowBook(bookId, readersCardId, dateTimeOffset);

        // Assert
        _bookLibraryRepository.Received(1).GetBook(bookId);
        _bookLibraryRepository.Received(1).BorrowBook(bookId, readersCardId, Arg.Any<DateTimeOffset>());
    }
}