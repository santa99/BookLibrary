using Contracts.Exceptions;

namespace Api.Exceptions;

/// <summary>
/// Request validation exception.
/// </summary>
public class RequestValidationException : Exception
{
    /// <summary>
    /// Associated error code.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    /// <inheritdoc />
    public RequestValidationException(string message) : base(message)
    {
        ErrorCode = ErrorCode.RequestValidationFailure;
    }
}