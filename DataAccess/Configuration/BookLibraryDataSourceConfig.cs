
namespace DataAccess.Configuration;

/// <summary>
/// Data source configuration.
/// </summary>
public class BookLibraryDataSourceConfig
{
    private readonly string _filePath = null!;
    private readonly string _validationSchema = null!;

    /// <summary>
    /// File path to data source like Library.xml
    /// </summary>
    public string FilePath
    {
        get => _filePath;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Invalid file path provided.");
            }
            _filePath = value;
        }
    }

    /// <summary>
    /// File path to XSD, validation schema for <see cref="FilePath"/>.
    /// </summary>
    public string ValidationSchema
    {
        get => _validationSchema;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Invalid file for validation schema.");
            }
            _validationSchema = value;
        }
    }
}