namespace Contracts;

public class LibraryModel
{
    /// <summary>
    /// Table of books.
    /// </summary>
    public List<BookModel> Books { get; set; } = new();
}