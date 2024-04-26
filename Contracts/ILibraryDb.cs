using DataAccess;

namespace Contracts;

public interface ILibraryDb
{
    void RemoveBook(int bookId);
    int InsertBook(string name, string author);
    void UpdateBook(int bookId, string name, string author);
    BookModel? GetBook(int bookId);
    List<BookModel> GetBooks();
    void BorrowBook(int bookId, string firstName, string lastName, DateTimeOffset from);
    void ReturnBook(int bookId);
}