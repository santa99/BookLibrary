using System.Xml.Serialization;
using DataAccess;

namespace Contracts;

[XmlRoot("Library")]
public class LibraryModel
{
    /// <summary>
    /// Table of books.
    /// </summary>
    [XmlElement("Book")]
    public List<BookModel> Books { get; set; } = new();
}