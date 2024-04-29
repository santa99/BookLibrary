namespace Api.Exceptions;

/// <summary>
/// Request validation exception.
/// </summary>
public class RequestValidationException : Exception
{
    public RequestValidationException(string message) : base(message)
    {
    }
}