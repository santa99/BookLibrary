using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Contracts;
using Contracts.Models;
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
    private readonly string _valueValidationSchema;
    private bool _schemaValidated;
    private readonly bool _isSchemaValidation;

    public BookLibraryDaoImpl(IOptions<BookLibraryDataSourceConfig> config)
    {
        var bookLibraryDataSourceConfig = config.Value;
        _libraryXml = bookLibraryDataSourceConfig.FilePath;
        _valueValidationSchema = bookLibraryDataSourceConfig.ValidationSchema;
        _isSchemaValidation = bookLibraryDataSourceConfig.IsSchemaValidation;
    }

    private void ValidateSchema()
    {
        if (_schemaValidated || !_isSchemaValidation)
        {
            return;
        }

        _schemaValidated = true;
        
        var schemas = new XmlSchemaSet();
        using var schema = new StreamReader(new FileStream(_valueValidationSchema, FileMode.Open));
        schemas.Add("", XmlReader.Create(schema));
        
        var xDocument = LoadXDocument();
        var stringBuilder = new StringBuilder();
        var invalidDocument = false;
        xDocument.Validate(schemas, (_, e) =>
        {
            stringBuilder.Append(e.Message).AppendLine();
            invalidDocument = true;
        });

        if (invalidDocument)
        {
            throw new InvalidOperationException($"Input library {_libraryXml} is corrupted or not in proper format.");
        }
    }

    public int Create(BookModel bookModel)
    {
        if (bookModel.Id > 0)
        {
            return -1;
        }

        var xDocument = LoadXDocument();
        bookModel.Id = GetNextBookId(xDocument);

        var bookElement = CreateBookElement(bookModel);

        xDocument.Element(LibraryElement)?.Add(bookElement);

        if (bookModel.Borrowed != null)
        {
            bookElement.Add(CreateBorrowedElement(bookModel.Borrowed));
        }

        StoreXDocument(xDocument);

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

    private XDocument LoadXDocument()
    {
        ValidateSchema();
        
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

    private static int MapBookId(XElement bookElement)
    {
        var bookIdAttribute = bookElement.Attribute(BookIdAttribute);
        if (bookIdAttribute == null)
        {
            throw new InvalidOperationException($"Missing element attribute '{BookIdAttribute}'");
        }

        return Convert.ToInt32(bookIdAttribute.Value);
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

    private static int GetNextBookId(XContainer xDocument)
    {
        var bookElements = xDocument.Descendants(BookElement);
        var nextBookId = bookElements.Select(MapBookId).Prepend(-1).Max();

        return nextBookId <= 0 ? 1 : nextBookId + 1;
    }
}