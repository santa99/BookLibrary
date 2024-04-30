using System.Net;
using Contracts.Exceptions;

namespace Api.Middleware.Exceptions.Mappers;

/// <summary>
///    Maps error codes in to the HTTP status codes.
/// </summary>
public interface IResponseStatusCodeMapper
{
    /// <summary>
    ///    Maps <paramref name="errorCode"/> into the <see cref="HttpStatusCode"/>s.
    /// </summary>
    /// <param name="errorCode"><see cref="ErrorCode"/></param>
    /// <returns><see cref="HttpStatusCode"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">is thrown in case of invalid error code mapping.</exception>
    HttpStatusCode Map(ErrorCode errorCode);
}

/// <inheritdoc />
public class ResponseStatusCodeMapper : IResponseStatusCodeMapper
{
    /// <inheritdoc />
    public HttpStatusCode Map(ErrorCode errorCode)
    {
        return errorCode switch
        {
            ErrorCode.Unexpected => HttpStatusCode.InternalServerError,
            ErrorCode.AuthenticationFailure => HttpStatusCode.Unauthorized,
            ErrorCode.RequestValidationFailure => HttpStatusCode.BadRequest,
            ErrorCode.BorrowedBook => HttpStatusCode.BadRequest,
            ErrorCode.BookNotFound => HttpStatusCode.NotFound,
            ErrorCode.ReadersCardNotFound => HttpStatusCode.NotFound,
            ErrorCode.BookNotBorrowed => HttpStatusCode.NotFound,
            _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
        };
    }
}