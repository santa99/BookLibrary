using System.Xml.Serialization;

namespace Contracts.Models;

[Serializable]
[XmlRoot("Library")]
public class Library
{
    /// <summary>
    /// Library with books
    /// </summary>
    [XmlElement("Book")]
    public List<BookModel> Books { get; set; } = new();
}