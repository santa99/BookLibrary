using System.Globalization;
using System.Xml.Serialization;

namespace Contracts.Models;

/// <summary>
/// Represents entity of the person who borrowed a book.
/// </summary>
public class BorrowModel
{
    private static readonly DateTimeFormatInfo DateTimeFormat = new CultureInfo("sk-SK").DateTimeFormat;

    /// <summary>
    /// First name who borrowed.
    /// </summary>
    [XmlElement("FirstName")]
    public string FirstName { get; set; }
    
    /// <summary>
    /// Last name who borrowed.
    /// </summary>
    [XmlElement("LastName")]
    public string LastName { get; set; }

    [XmlIgnore]
    public DateTimeOffset From { get; set; }
    
    /// <summary>
    /// Date when the book was last time borrowed.
    /// </summary>
    [XmlElement("From")]
    public string FromXml
    {
        get => From.ToString(DateTimeFormat);
        set
        {
            if (DateTimeOffset.TryParse(value, DateTimeFormat, DateTimeStyles.None,
                    out var parsed))
            {
                From = parsed;
            }
        }
    }
}