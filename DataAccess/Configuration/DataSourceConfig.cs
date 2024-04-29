
namespace DataAccess.Configuration;

/// <summary>
/// Data source configuration.
/// </summary>
public class DataSourceConfig
{
    private readonly string _connectionString = null!;

    /// <summary>
    /// File path to data source like Library.xml
    /// </summary>
    public string ConnectionString
    {
        get => _connectionString;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Invalid connection string provided.");
            }
            _connectionString = value;
        }
    }
}