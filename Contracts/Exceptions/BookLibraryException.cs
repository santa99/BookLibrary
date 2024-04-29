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