namespace Contracts;

/// <summary>
/// Represents entity of the person who borrowed a book.
/// </summary>
public class BorrowModel
{
    /// <summary>
    /// First name who borrowed.
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    /// Last name who borrowed.
    /// </summary>
    public string LastName { get; set; }
    
    /// <summary>
    /// Date when the book was last time borrowed.
    /// </summary>
    public DateTimeOffset From { get; set; }
}