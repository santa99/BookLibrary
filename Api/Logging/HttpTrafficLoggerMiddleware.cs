using System.Diagnostics;
using System.Net;

namespace Api.Logging;

/// <summary>
/// Class <see cref="HttpTrafficLoggerMiddleware"/> is middleware handling logs for all requests and their
/// corresponding responses.
/// </summary>
public class HttpTrafficLoggerMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly IHttpRequestLogger _httpRequestLogger;
    private readonly ILogger _logger;
    private const string Empty = "Empty";

    /// <summary />
    public HttpTrafficLoggerMiddleware(
        RequestDelegate requestDelegate,
        IHttpRequestLogger httpRequestLogger,
        ILogger<HttpTrafficLoggerMiddleware> logger)
    {
        _requestDelegate = requestDelegate;
        _httpRequestLogger = httpRequestLogger;
        _logger = logger;
    }

    /// <summary>
    /// Invoke this middleware.
    /// </summary>
    /// <param name="httpContext"><see cref="HttpContext"/></param>
    public async Task Invoke(HttpContext httpContext)
    {
        await InvokeWithLogging(httpContext);
    }

    private async Task InvokeWithLogging(HttpContext httpContext)
    {
        var stopWatch = Stopwatch.StartNew();
        var dateTime = DateTime.UtcNow;

        HttpStatusCode statusCode = 0;
        var reqBody = ReadBody(httpContext);
        string? resBody = null;

        try
        {
            resBody = await InvokeWithResponseBodyRetrieval(httpContext);

            statusCode = (HttpStatusCode)httpContext.Response.StatusCode;
        }
        finally
        {
            _httpRequestLogger.Log(
                httpContext.TraceIdentifier,
                statusCode,
                httpContext.Request?.Method ?? Empty,
                httpContext.Request?.Host.Value ?? Empty,
                httpContext.Request?.Path ?? Empty,
                httpContext.Request?.QueryString.Value ?? Empty,
                httpContext.Request?.Headers ?? new HeaderDictionary(),
                reqBody ?? Empty,
                httpContext.Response?.Headers ?? new HeaderDictionary(),
                resBody ?? Empty,
                stopWatch.Elapsed.Milliseconds,
                dateTime
            );
        }
    }

    private string? ReadBody(HttpContext httpContext)
    {
        var originalStream = httpContext.Request.Body;
        using var rewindableStream = new MemoryStream();

        httpContext.Request.Body = originalStream;
        try
        {
            if (!rewindableStream.CanSeek) return null;
            var responseBodyText = ReadAndRewind(rewindableStream);
            return responseBodyText;
        }
        finally
        {
            httpContext.Request.Body = originalStream;
        }
    }

    private async Task<string?> InvokeWithResponseBodyRetrieval(HttpContext httpContext)
    {
        var originalStream = httpContext.Response.Body;
        using var rewindableStream = new MemoryStream();

        httpContext.Response.Body = rewindableStream;
        try
        {
            await _requestDelegate(httpContext);

            if (!rewindableStream.CanSeek)
            {
                _logger.Log(LogLevel.Error,
                    "Can't read the content from rewindable stream. {TraceId}, {Host}, {Path}, {QueryParams}",
                    httpContext.TraceIdentifier,
                    httpContext.Request?.Host.Value, httpContext.Request?.Path.Value,
                    httpContext.Request?.QueryString.Value
                );
                return null;
            }

            var responseBodyText = ReadAndRewind(rewindableStream);
            await rewindableStream.CopyToAsync(originalStream);
            return responseBodyText;
        }
        finally
        {
            httpContext.Response.Body = originalStream;
        }
    }

    private static string ReadAndRewind(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var readToEnd = new StreamReader(stream).ReadToEnd();
        stream.Seek(0, SeekOrigin.Begin);
        return readToEnd;
    }
}