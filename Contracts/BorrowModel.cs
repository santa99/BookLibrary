using System.Xml.Serialization;

namespace DataAccess;

/// <summary>
/// Represents entity of the person who borrowed a book.
/// </summary>
public class BorrowDto
{
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
    
    /// <summary>
    /// Date when the book was last time borrowed.
    /// </summary>
    [XmlElement("From")]
    public DateTimeOffset From { get; set; }
}