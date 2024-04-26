using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Contracts;
using DataAccess;
using NSubstitute;
using Xunit;

namespace Api.Tests;

public class BookCommandTests
{
    protected readonly IFixture Fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
    private readonly IBookCommand _sut;
    private readonly IBookLibraryRepository _bookLibraryRepository;
    private readonly IReadersInfoRepository _readersInfoRepository;

    public BookCommandTests()
    {
        _bookLibraryRepository = Fixture.Create<IBookLibraryRepository>();
        _readersInfoRepository = Fixture.Create<IReadersInfoRepository>();
        _sut = new BookCommand(_bookLibraryRepository, _readersInfoRepository);
    }

    [Fact]
    public void BorrowBook_Ok()
    {
        // Arrange
        var bookId = Fixture.Create<int>();
        var readersCardId = Fixture.Create<int>();
        var dateTimeOffset = Fixture.Create<DateTimeOffset>();
        var bookModel = Fixture.Create<BookModel>();
        var readersInfo = Fixture.Create<ReaderInfo>();
        bookModel.BorrowedBy = null;
        _bookLibraryRepository.GetBook(bookId)
            .Returns(bookModel);
        _readersInfoRepository.GetReadersInfo(readersCardId)
            .Returns(readersInfo);
        
        // Act
        _sut.BorrowBook(bookId, readersCardId, dateTimeOffset);

        // Assert
        _bookLibraryRepository.Received(1).GetBook(bookId);
        _readersInfoRepository.Received(1).GetReadersInfo(readersCardId);
        _bookLibraryRepository.Received(1).BorrowBook(bookId, readersInfo.Name, readersInfo.LastName, Arg.Any<DateTimeOffset>());
    }

}