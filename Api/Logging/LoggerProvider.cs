using Serilog;
using Serilog.Extensions.Logging;

namespace Api.Logging;

/// <summary>
///  Book library logger provides loggers for HTTP logging.
/// </summary>
public interface IBookLibraryLoggerProvider
{
    /// <summary>
    /// In logger
    /// </summary>
    IHttpRequestLogger InLogger { get; }
}

/// <inheritdoc />
public class BookLibraryLoggerProvider : IBookLibraryLoggerProvider
{
    /// <inheritdoc />
    public IHttpRequestLogger InLogger { get; }

    /// <summary />
    public BookLibraryLoggerProvider()
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.Enrich.FromLogContext();
        loggerConfiguration.MinimumLevel.Verbose();
        loggerConfiguration.WriteTo.File("C:/Logs/Http/intraffic.txt", outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        var internalLogger = loggerConfiguration.CreateLogger();
        var logger = new SerilogLoggerProvider(internalLogger).CreateLogger("Http-In");
        
        InLogger = new HttpRequestLogger(logger);
    }
}