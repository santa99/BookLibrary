namespace Contracts.Exceptions;

public class BookLibraryException : Exception
{
    /// <summary>
    /// Associated error code.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    public BookLibraryException(string message, ErrorCode errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class BookNotFoundException : BookLibraryException
{
    public BookNotFoundException(int bookId) : base($"Requested bookId: {bookId} does not exist.", ErrorCode.BookNotFound)
    {
    }
}