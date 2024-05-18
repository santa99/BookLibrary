using System.Xml.Serialization;

namespace Contracts.Models;

/// <summary>
/// Model for the book
/// </summary>
public class BookModel
{
    /// <summary>
    /// Genuine book id.
    /// </summary>
    [XmlAttribute("id")]
    public int Id { get; set; }

    /// <summary>
    /// Name of the title.
    /// </summary>
    [XmlElement("Name")]
    public string Name { get; set; }

    /// <summary>
    /// Author of the book.
    /// </summary>
    [XmlElement("Author")]
    public string Author { get; set; }
    
    /// <summary>
    /// Borrowed by.
    /// </summary>
    [XmlElement("Borrowed")]
    public BorrowModel? Borrowed { get; set; }

    public static BookModel FromBook(BookModel bookModel)
    {
        return new BookModel
        {
            Author = bookModel.Author,
            Id = bookModel.Id,
            Borrowed = bookModel.Borrowed,
            Name = bookModel.Name
        };
    }
}