using System.Net;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Api.Logging;

/// <inheritdoc />
public class HttpRequestLogger : IHttpRequestLogger
{
    private readonly ILogger _logger;

    /// <summary>
    /// Logger. 
    /// </summary>
    /// <param name="logger">Customized logger.</param>
    public HttpRequestLogger(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void Log(
        string traceId,
        HttpStatusCode statusCode,
        string method, string host, string path,
        string query,
        IHeaderDictionary requestHeaders,
        string requestBody,
        IHeaderDictionary responseHeaders,
        string responseBody,
        long durationMs,
        DateTime requestStartDateTime)
    {
        var headersReq = Headers(requestHeaders);
        var headersRes = Headers(responseHeaders);
        using (_logger.BeginScope<IEnumerable<KeyValuePair<string, object>>>(new Dictionary<string, object>
               {
                   { "tid", traceId },
                   { "statusCode", statusCode },
                   { "startDate", requestStartDateTime },
                   { "timeSpent", durationMs },
                   { "method", method },
                   { "host", host },
                   { "path", path },
                   { "query", query },
                   { "reqHeaders", headersReq },
                   { "reqBody", requestBody },
                   { "resHeaders", headersRes },
                   { "resBody", responseBody },
               }))
        {
            _logger.LogInformation("Trace:[{tid}], Status:{statusCode}, When: {startDate}, Taken: {timeSpent}, Method: {method}, Path: {path}, QueryParams: {query}" +
                                   "\n\rRequest-Headers: '{reqHeaders}'\n\rRequest-Body: '{reqBody}' \n\r" +
                                   "\n\rResponse-Headers: '{resHeaders}'\n\rResponse-Body: '{resBody}' \n\r"
            );
        }
    }

    private string Headers(IHeaderDictionary headerDictionary)
    {
        return BuildHeadersString(
            headerDictionary
                .Select((Func<KeyValuePair<string, StringValues>, KeyValuePair<string, string>>)
                    (h =>
                        new KeyValuePair<string, string>(
                            h.Key,
                            string.Join(",", (IEnumerable<string>)(string[])h.Value ?? Enumerable.Empty<string>())
                        ))
                )
        );
    }

    private static string? BuildHeadersString(IEnumerable<KeyValuePair<string, string>> headers)
    {
        var flag = false;
        var stringBuilder = new StringBuilder();
        foreach (var keyValuePair in headers)
        {
            stringBuilder.Append(keyValuePair.Key);
            stringBuilder.Append(": ");
            stringBuilder.Append(keyValuePair.Value);
            stringBuilder.Append("\r\n");
            flag = true;
        }

        return flag ? stringBuilder.ToString() : null;
    }
}