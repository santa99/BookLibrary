namespace Contracts.Enums;

/// <summary>
/// Enumeration <see cref="BookState"/> represents all possible state of book.
/// </summary>
public enum BookState
{
    /// <summary>
    /// Retrieves all books.
    /// </summary>
    All = -1,
    
    /// <summary>
    /// Retrieves only available books.
    /// </summary>
    Free = 0,
    
    /// <summary>
    /// Retrieves borrowed books only.
    /// </summary>
    Borrowed = 1
}