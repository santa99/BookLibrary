namespace Contracts.Exceptions;

public enum ErrorCode
{
    /// <summary>
    /// Unexpected failure.
    /// </summary>
    Unexpected = 1,

    /// <summary>
    /// Error code on authentication failure.
    /// </summary>
    AuthenticationFailure = 2,

    /// <summary>
    /// Request validation has failed.
    /// </summary>
    RequestValidationFailure = 3,

    /// <summary>
    /// Borrowed book.
    /// </summary>
    BorrowedBook = 4,

    /// <summary>
    /// Book not found
    /// </summary>
    BookNotFound = 5,
    
    /// <summary>
    /// Readers card not found
    /// </summary>
    ReadersCardNotFound = 6,
    
    /// <summary>
    /// Borrowed book.
    /// </summary>
    BookNotBorrowed = 7,
}