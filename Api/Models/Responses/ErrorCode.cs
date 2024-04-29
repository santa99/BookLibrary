namespace Api.Models.Responses;

public enum ErrorCode
{
    /// <summary>
    /// Unexpected failure.
    /// </summary>
    UNEXPECTED = 1,

    /// <summary>
    /// Error code on authentication failure.
    /// </summary>
    AUTHENTICATION_FAILURE = 2,

    /// <summary>
    /// Request validation has failed.
    /// </summary>
    REQUEST_VALIDATION_FAILURE = 3,
}