using System.Net.Mime;
using System.Security.Authentication;
using System.Text.Json;
using Api.Exceptions;
using Api.Middleware.Exceptions.Mappers;
using Contracts.Exceptions;

namespace Api.Middleware.Exceptions;

/// <summary>
/// General error middleware.
/// </summary>
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IResponseStatusCodeMapper _responseStatusCodeMapper;
    private readonly IErrorResponseMapper _errorResponseMapper;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;


    public ErrorHandlerMiddleware(
        RequestDelegate next,
        IResponseStatusCodeMapper responseStatusCodeMapper,
        IErrorResponseMapper errorResponseMapper,
        ILogger<ErrorHandlerMiddleware> logger
    )
    {
        _next = next;
        _responseStatusCodeMapper = responseStatusCodeMapper;
        _errorResponseMapper = errorResponseMapper;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception: {Exception}{NewLine}StackTrace: {StackTrace}", exception.Message,
                Environment.NewLine, exception.StackTrace);


            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;

            var errorResponse = exception switch
            {
                BookLibraryException e => _errorResponseMapper.MapBookLibraryException(e),
                RequestValidationException e => _errorResponseMapper.MapRequestValidationException(e),
                AuthenticationException e => _errorResponseMapper.MapAuthenticationException(e),
                ArgumentOutOfRangeException e => _errorResponseMapper.MapUnhandledException(e),
                _ => _errorResponseMapper.MapUnhandledException(exception)
            };

            response.StatusCode = (int)_responseStatusCodeMapper.Map((ErrorCode)errorResponse.Id);

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}