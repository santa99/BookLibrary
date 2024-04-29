using System.Net;
using Api.Models.Responses;

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
            ErrorCode.UNEXPECTED => HttpStatusCode.InternalServerError,
            ErrorCode.AUTHENTICATION_FAILURE => HttpStatusCode.Unauthorized,
            ErrorCode.REQUEST_VALIDATION_FAILURE => HttpStatusCode.BadRequest,
            _ => throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null)
        };
    }
}