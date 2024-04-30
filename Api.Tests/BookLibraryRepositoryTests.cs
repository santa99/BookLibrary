using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Contracts;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Models;
using DataAccess;
using NSubstitute;
using Xunit;

namespace Api.Tests;

public class BookLibraryRepositoryTests : TestBase
{
    private readonly IBookLibraryDao _bookLibraryDao;
    private readonly IReadersInfoDao _readersInfoDao;
    private readonly BookLibraryRepository _sut;

    public BookLibraryRepositoryTests()
    {
        _bookLibraryDao = Fixture.Create<IBookLibraryDao>();
        _readersInfoDao = Fixture.Create<IReadersInfoDao>();
        _sut = new BookLibraryRepository(_bookLibraryDao, _readersInfoDao);
    }

    [Fact]
    public async Task BorrowBook_InvalidBookId_ThrowsBookLibraryException()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        bookModel.Borrowed = null;
        var readersCardId = Fixture.Create<int>();
        var dateTimeOffset = Fixture.Create<DateTimeOffset>();
        _bookLibraryDao.Read(bookModel.Id).Returns((BookModel)null!);

        // Act
        var borrowABook = async () =>
            await _sut.BorrowBook(bookModel.Id, readersCardId, dateTimeOffset, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BookLibraryException>(async () => await borrowABook());
    }

    [Fact]
    public async Task BorrowBook_TwiceTheSameBook_ThrowsBookLibraryException()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        bookModel.Borrowed = null;
        var readersCardId = Fixture.Create<int>();
        var dateTimeOffset = Fixture.Create<DateTimeOffset>();
        _bookLibraryDao.Read(bookModel.Id).Returns(bookModel);

        // Act
        var borrowABook = async () =>
            await _sut.BorrowBook(bookModel.Id, readersCardId, dateTimeOffset, CancellationToken.None);
        await borrowABook();

        // Assert
        await Assert.ThrowsAsync<BookLibraryException>(async () => await borrowABook());
        _readersInfoDao.Received(1).Read(Arg.Any<int>());
        _bookLibraryDao.Received(1).Update(bookModel);
    }
    
    [Fact]
    public async Task BorrowBook_ReadersInfoDoesExist_ThrowsBookLibraryException()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        bookModel.Borrowed = null;
        var readersCardId = Fixture.Create<int>();
        var dateTimeOffset = Fixture.Create<DateTimeOffset>();
        _bookLibraryDao.Read(bookModel.Id).Returns(bookModel);
        _readersInfoDao.Read(readersCardId).Returns((ReadersInfo)null!);

        // Act
        var borrowABook = async () =>
            await _sut.BorrowBook(bookModel.Id, readersCardId, dateTimeOffset, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BookLibraryException>(async () => await borrowABook());
        _bookLibraryDao.Received(1).Read(bookModel.Id);
        _readersInfoDao.Received(1).Read(readersCardId);
    }
    
    [Fact]
    public async Task ReturnBook_BookIdInvalid_ThrowsBookLibraryException()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        bookModel.Borrowed = null;
        _bookLibraryDao.Read(bookModel.Id).Returns((BookModel) null!);

        // Act
        var returnBook = async () =>
            await _sut.ReturnBook(bookModel.Id, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BookLibraryException>(async () => await returnBook());
        _bookLibraryDao.Received(1).Read(bookModel.Id);
    }
    
    [Fact]
    public async Task ReturnBook_BookIsNotBorrowed_ThrowsBookLibraryException()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        bookModel.Borrowed = null;
        _bookLibraryDao.Read(bookModel.Id).Returns(bookModel);

        // Act
        var returnBook = async () =>
            await _sut.ReturnBook(bookModel.Id, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BookLibraryException>(async () => await returnBook());
        _bookLibraryDao.Received(1).Read(bookModel.Id);
    }
    
    [Fact]
    public async Task ReturnBook_Ok()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        _bookLibraryDao.Read(bookModel.Id).Returns(bookModel);

        // Act
        var returnBook = await _sut.ReturnBook(bookModel.Id, CancellationToken.None);

        // Assert
        Assert.Equal(bookModel.Id, returnBook);
        _bookLibraryDao.Received(1).Read(bookModel.Id);
        _bookLibraryDao.Received(1).Update(bookModel);
    }
    
    [Fact]
    public async Task UpdateBookDetails_Ok()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        _bookLibraryDao.Read(bookModel.Id).Returns(bookModel);

        // Act
        await _sut.UpdateBookDetails(bookModel.Id,bookModel.Name, bookModel.Author, CancellationToken.None);

        // Assert
        _bookLibraryDao.Received(1).Read(bookModel.Id);
        _bookLibraryDao.Received(1).Update(Arg.Is<BookModel>(bM => 
            bM.Id == bookModel.Id &&
            bM.Name == bookModel.Name &&
            bM.Author == bookModel.Author));
    }
    
    [Fact]
    public async Task UpdateBookDetails_InvalidBookId_ThrowsBookLibraryException()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        _bookLibraryDao.Read(bookModel.Id).Returns((BookModel) null!);

        // Act
        var updateBookDetails = async () => await _sut.UpdateBookDetails(bookModel.Id,bookModel.Name, bookModel.Author, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<BookLibraryException>(async () => await updateBookDetails());
        _bookLibraryDao.Received(1).Read(bookModel.Id);
    }
    
    [Fact]
    public async Task GetBook_InvalidBookId_ReturnsNull()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        _bookLibraryDao.Read(bookModel.Id).Returns((BookModel) null!);

        // Act
        var book = await _sut.GetBook(bookModel.Id, CancellationToken.None);

        // Assert
        Assert.Null(book);
        _bookLibraryDao.Received(1).Read(bookModel.Id);
    }
    
    [Fact]
    public async Task GetBook_Ok()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        _bookLibraryDao.Read(bookModel.Id).Returns(bookModel);

        // Act
        var book = await _sut.GetBook(bookModel.Id, CancellationToken.None);

        // Assert
        Assert.Equal(bookModel, book);
        _bookLibraryDao.Received(1).Read(bookModel.Id);
    }
    
    [Fact]
    public async Task AddNewBook_Ok()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();
        var name = bookModel.Name;
        var author = bookModel.Author;
        _bookLibraryDao.Create(Arg.Any<BookModel>()).Returns(bookModel.Id);

        // Act
        var bookId = await _sut.AddNewBook(name, author, CancellationToken.None);

        // Assert
        Assert.Equal(bookModel.Id, bookId);
        _bookLibraryDao.Received(1).Create(Arg.Any<BookModel>());
    }
    
    [Fact]
    public async Task RemoveBook_Ok()
    {
        // Arrange
        var bookModel = Fixture.Create<BookModel>();

        // Act
        await _sut.RemoveBook(bookModel.Id, CancellationToken.None);

        // Assert
        _bookLibraryDao.Received(1).Delete(bookModel.Id);
    }

    private static IEnumerable<object[]> GetStateAndData()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization
            { ConfigureMembers = true });

        var borrowedBook = fixture.Create<BookModel>();
        var freeBook = fixture.Create<BookModel>();
        borrowedBook.Borrowed = new BorrowModel();
        freeBook.Borrowed = null;

        yield return new object[]
        {
            BookState.All, new List<BookModel>
            {
                borrowedBook, freeBook
            }, new List<BookModel>
            {
                borrowedBook, freeBook
            }
        };
        yield return new object[]
        {
            BookState.Borrowed, new List<BookModel>
            {
                borrowedBook, freeBook
            },
            new List<BookModel>
            {
                borrowedBook
            }
        };
        yield return new object[]
        {
            BookState.Free, new List<BookModel>
            {
                freeBook, borrowedBook
            },
            new List<BookModel>
            {
                freeBook
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetStateAndData))]
    public async Task ListBooks_Ok(BookState bookState, List<BookModel> returnedBooks, List<BookModel> expectedBooks)
    {
        // Arrange
        _bookLibraryDao.GetBooks().Returns(returnedBooks);
        
        // Act
        var actualBooks = await _sut.ListBooks(bookState, CancellationToken.None);
        
        // Assert
        Assert.True(actualBooks.Count > 0);
        _bookLibraryDao.Received(1).GetBooks();
        Assert.Equal(expectedBooks.Count, actualBooks.Count); 
        Assert.True(expectedBooks.SequenceEqual(actualBooks));
    }
    
    [Theory]
    [MemberData(nameof(GetStateAndData))]
    public async Task ListBooks_Start_Count_Ok(BookState bookState, List<BookModel> returnedBooks, List<BookModel> expectedBooks)
    {
        // Arrange
        _bookLibraryDao.GetBooks().Returns(returnedBooks);
        var expectedCount = 1;
        
        // Act
        var actualBooks = await _sut.ListBooks(bookState, CancellationToken.None, expectedCount);
        
        // Assert
        _bookLibraryDao.Received(1).GetBooks();
        Assert.True(actualBooks.Count == expectedCount);
        Assert.Contains(actualBooks, model => model.Equals(expectedBooks.First()));
    }
}