using System.Net;

namespace Api.Logging;

/// <summary>
/// Contract for logging request and their corresponding
/// responses together.
/// </summary>
public interface IHttpRequestLogger
{
    /// <summary>
    ///  Log message with advanced parameters.
    /// </summary>
    /// <param name="traceId">Trace identifier.</param>
    /// <param name="statusCode">Response message status code.</param>
    /// <param name="method"><see cref="HttpMethod"/> on request.</param>
    /// <param name="host">Host address.</param>
    /// <param name="path">Path</param>
    /// <param name="query">Query parameters if available.</param>
    /// <param name="requestHeaders">Request headers.</param>
    /// <param name="requestBody">Request body.</param>
    /// <param name="responseHeaders">Response headers.</param>
    /// <param name="responseBody">Response body.</param>
    /// <param name="durationMs">Time spent with request.</param>
    /// <param name="requestStartDateTime">Start date time.</param>
    void Log(
        string traceId,
        HttpStatusCode statusCode,
        string method,
        string host,
        string path,
        string query,
        IHeaderDictionary requestHeaders,
        string requestBody,
        IHeaderDictionary responseHeaders,
        string responseBody,
        long durationMs,
        DateTime requestStartDateTime);
}