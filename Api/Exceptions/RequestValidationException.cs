using Api.Models.Responses;

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
    
    public RequestValidationException(string message) : base(message)
    {
        ErrorCode = ErrorCode.REQUEST_VALIDATION_FAILURE;
    }
}