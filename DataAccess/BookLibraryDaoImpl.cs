using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Contracts;
using DataAccess.Configuration;
using Microsoft.Extensions.Options;

namespace DataAccess;

public class BookLibraryDaoImpl : IBookLibraryDao
{
    private readonly string _libraryXml;
    private const string LibraryElement = "Library";
    private const string BookElement = "Book";
    private const string BookNameElement = "Name";
    private const string BookAuthorElement = "Author";
    private const string BorrowedElement = "Borrowed";
    private const string BorrowedFirstNameElement = "FirstName";
    private const string BorrowedLastNameElement = "LastName";
    private const string BorrowedFromElement = "From";
    private const string BookIdAttribute = "id";
    private static readonly DateTimeFormatInfo DateTimeFormat = new CultureInfo("sk-SK").DateTimeFormat;

    public BookLibraryDaoImpl(IOptions<BookLibraryDataSourceConfig> config)
    {
        _libraryXml = config.Value.FilePath;
    }

    public int Create(BookModel bookModel)
    {
        if (bookModel.Id > 0)
        {
            return -1;
        }

        bookModel.Id = GetNextBookId(LoadXmlDocument());
        var xmlDoc = LoadXDocument();

        var bookElement = CreateBookElement(bookModel);

        xmlDoc.Element(LibraryElement)?.Add(bookElement);

        if (bookModel.Borrowed != null)
        {
            bookElement.Add(CreateBorrowedElement(bookModel.Borrowed));
        }

        StoreXDocument(xmlDoc);

        return bookModel.Id;
    }

    public BookModel? Read(int bookId)
    {
        if (bookId <= 0)
        {
            return null;
        }

        var xDocument = LoadXDocument();
        var bookByIdElement = GetBookByIdElement(xDocument.Descendants(BookElement), bookId);
        return bookByIdElement == null ? null : Map(bookByIdElement);
    }

    public void Update(BookModel bookModel)
    {
        var xDocument = LoadXDocument();
        var bookByIdElement = GetBookByIdElement(xDocument.Descendants(BookElement), bookModel.Id);

        if (bookByIdElement == null)
        {
            return;
        }

        SetElementValue(bookByIdElement, BookNameElement, bookModel.Name);
        SetElementValue(bookByIdElement, BookAuthorElement, bookModel.Author);
        if (bookModel.Borrowed != null)
        {
            if (bookByIdElement.Element(BorrowedElement) != null)
            {
                bookByIdElement.Element(BorrowedElement)?.Remove();
            }

            bookByIdElement.Add(CreateBorrowedElement(bookModel.Borrowed));
        }
        else
        {
            bookByIdElement.Element(BorrowedElement)?.Remove();
        }

        StoreXDocument(xDocument);
    }

    public void Delete(int bookId)
    {
        if (bookId <= 0)
        {
            return;
        }

        var xDocument = LoadXDocument();
        var bookByIdElement = GetBookByIdElement(xDocument.Descendants(BookElement), bookId);

        if (bookByIdElement == null)
        {
            return;
        }

        bookByIdElement.Remove();
        StoreXDocument(xDocument);
    }


    public List<BookModel> GetBooks()
    {
        var xDocument = LoadXDocument();
        var bookElements = xDocument.Descendants(BookElement);

        return bookElements.Select(Map).ToList();
    }

    private static XElement? GetBookByIdElement(IEnumerable<XElement> bookElements, int bookId)
    {
        return bookElements.FirstOrDefault(element =>
        {
            var bookIdAttribute = element.Attribute(BookIdAttribute);
            if (bookIdAttribute == null)
            {
                return false;
            }

            return int.TryParse(bookIdAttribute.Value, out var validBookIdNumber) && validBookIdNumber == bookId;
        });
    }

    private XmlDocument LoadXmlDocument()
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.Load(_libraryXml);

        return xmlDocument;
    }

    private XDocument LoadXDocument()
    {
        return XDocument.Load(_libraryXml);
    }

    private void StoreXDocument(XDocument xDocument)
    {
        xDocument.Save(_libraryXml);
    }

    private static XElement CreateBookElement(BookModel bookModel)
    {
        return new XElement(BookElement,
            new XAttribute(BookIdAttribute, bookModel.Id),
            new XElement(BookNameElement, bookModel.Name),
            new XElement(BookAuthorElement, bookModel.Author)
        );
    }

    private static XElement CreateBorrowedElement(BorrowModel borrowModel)
    {
        return new XElement(BorrowedElement,
            new XElement(BorrowedFirstNameElement, borrowModel.FirstName),
            new XElement(BorrowedLastNameElement, borrowModel.LastName),
            new XElement(BorrowedFromElement, borrowModel.From)
        );
    }

    private static void SetElementValue(XElement elementToUpdate, string elementName, string? elementValue)
    {
        var bookNameElement = elementToUpdate.Element(elementName);
        if (bookNameElement != null)
        {
            bookNameElement.Value = elementValue!;
        }
    }

    private static BookModel Map(XElement bookElement)
    {
        var bookIdAttribute = bookElement.Attribute(BookIdAttribute);
        if (bookIdAttribute == null)
        {
            throw new InvalidOperationException($"Missing element attribute '{BookIdAttribute}'");
        }

        var bookNameElement = bookElement.Element(BookNameElement);
        if (bookNameElement == null)
        {
            throw new InvalidOperationException($"Missing element '{BookNameElement}'");
        }

        var bookAuthorElement = bookElement.Element(BookAuthorElement);
        if (bookAuthorElement == null)
        {
            throw new InvalidOperationException($"Missing element '{BookAuthorElement}'");
        }

        var borrowedElement = bookElement.Element(BorrowedElement);
        BorrowModel? borrowed = null;
        if (borrowedElement != null)
        {
            var borrowedFromElement = borrowedElement.Element(BorrowedFromElement);
            if (borrowedFromElement == null)
            {
                throw new InvalidOperationException($"Missing element '{BorrowedFromElement}'");
            }

            var borrowedFirstNameElement = borrowedElement.Element(BorrowedFirstNameElement);
            if (borrowedFirstNameElement == null)
            {
                throw new InvalidOperationException($"Missing element '{BorrowedFirstNameElement}'");
            }

            var borrowedLastNameElement = borrowedElement.Element(BorrowedLastNameElement);
            if (borrowedLastNameElement == null)
            {
                throw new InvalidOperationException($"Missing element '{BorrowedLastNameElement}'");
            }

            borrowed = new BorrowModel
            {
                From = DateTimeOffset.Parse(borrowedFromElement.Value, DateTimeFormat),
                FirstName = borrowedFirstNameElement.Value,
                LastName = borrowedLastNameElement.Value
            };
        }

        return new BookModel
        {
            Id = Convert.ToInt32(bookIdAttribute.Value),
            Name = bookNameElement.Value,
            Author = bookAuthorElement.Value,
            Borrowed = borrowed
        };
    }

    private static int GetNextBookId(XmlDocument xmlDocument)
    {
        var elementsByTagName = xmlDocument.GetElementsByTagName(BookElement);
        var nextBookId = -1;
        foreach (var book in elementsByTagName)
        {
            if (book is not XmlNode bookXmlNode)
            {
                continue;
            }

            var xmlAttribute = bookXmlNode.Attributes?[BookIdAttribute];
            if (xmlAttribute != null && int.TryParse(xmlAttribute.InnerText, out var validBookIdNumber) &&
                validBookIdNumber > nextBookId)
                nextBookId = validBookIdNumber;
        }

        return nextBookId <= 0 ? 1 : nextBookId + 1;
    }
}