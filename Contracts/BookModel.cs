namespace Contracts;

/// <summary>
/// Model for the book
/// </summary>
public class BookModel
{
    /// <summary>
    /// Genuine book id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the title.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Author of the book.
    /// </summary>
    public string Author { get; set; }
    
    /// <summary>
    /// Borrowed by.
    /// </summary>
    public BorrowModel? Borrowed { get; set; }
}