using System.Security.Authentication;
using Api.Exceptions;
using Api.Models.Responses;
using Contracts.Exceptions;

namespace Api.Middleware.Exceptions.Mappers;

/// <summary>
///    Maps well known adapter exceptions into <see cref="ErrorCodeModel"/>.
/// </summary>
public interface IErrorResponseMapper
{
    /// <summary>
    ///   Maps <see cref="BookLibraryException"/> into the <see cref="ErrorCodeModel"/>.
    /// </summary>
    /// <param name="exception"><see cref="BookLibraryException"/></param>
    /// <returns><see cref="ErrorCodeModel"/></returns>
    ErrorCodeModel MapBookLibraryException(BookLibraryException exception);

    /// <summary>
    ///   Maps general <paramref name="exception"/> into the <see cref="ErrorCodeModel"/>.
    /// </summary>
    /// <param name="exception"><see cref="Exception"/></param>
    /// <returns><see cref="ErrorCodeModel"/></returns>
    ErrorCodeModel MapUnhandledException(Exception exception);

    /// <summary>
    ///   Maps any kind of auth exceptions into the <see cref="ErrorCodeModel"/>.
    /// </summary>
    /// <param name="exception"><see cref="AuthenticationException"/></param>
    /// <returns><see cref="ErrorCodeModel"/></returns>
    ErrorCodeModel MapAuthenticationException(AuthenticationException exception);

    /// <summary>
    ///   Maps <see cref="RequestValidationException"/> into the <see cref="ErrorCodeModel"/>.
    /// </summary>
    /// <param name="exception"><see cref="RequestValidationException"/></param>
    /// <returns><see cref="ErrorCodeModel"/></returns>
    ErrorCodeModel MapRequestValidationException(RequestValidationException exception);
}

/// <inheritdoc />
public class ErrorResponseMapper : IErrorResponseMapper
{
    public ErrorCodeModel MapBookLibraryException(BookLibraryException exception)
    {
        return new ErrorCodeModel(
            (int)exception.ErrorCode,
            exception.Message,
            "Book library exception."
        );
    }

    /// <inheritdoc />
    public ErrorCodeModel MapUnhandledException(Exception exception)
    {
        return new ErrorCodeModel(
            (int)ErrorCode.Unexpected,
            exception.Message,
            "Unexpected exception."
        );
    }

    /// <inheritdoc />
    public ErrorCodeModel MapAuthenticationException(AuthenticationException exception)
    {
        return new ErrorCodeModel(
            (int)ErrorCode.AuthenticationFailure,
            exception.Message,
            "Authentication has failed."
        );
    }

    /// <inheritdoc />
    public ErrorCodeModel MapRequestValidationException(RequestValidationException exception)
    {
        return new ErrorCodeModel(
            (int)exception.ErrorCode,
            exception.Message,
            "Wrong parameters in the request model."
        );
    }
}