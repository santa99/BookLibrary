using Api.Models.Responses;

namespace Api.Exceptions;

public class BookLibraryException : Exception
{
    /// <summary>
    /// Associated error code.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    protected BookLibraryException(string message, ErrorCode errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}