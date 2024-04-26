using System.Globalization;
using System.Xml;
using Contracts;

namespace DataAccess;

public class LibraryDb : ILibraryDb
{
    private LibraryModel? _library;
    private XmlDocument? _doc;
    private const string LibraryFile = @"Library.xml";

    private void LoadLibrary(string webRootPath)
    {
        if (_doc != null)
        {
            return;
        }

        _doc = new XmlDocument();
        _doc.Load(webRootPath);
    }

    private void StoreLibrary(string webRootPath)
    {
        _doc?.Save(webRootPath);
    }

    private LibraryModel Library => _library ??= new LibraryModel();

    public void RemoveBook(int bookId)
    {
        LoadLibrary(LibraryFile);

        XmlNodeList bookNodes = _doc.SelectNodes("/Library/Book");
        for (var i = 0; i < bookNodes.Count; i++)
        {
            XmlNode? bookNode = bookNodes[i];
            var documentBookId = int.Parse(bookNode.Attributes["id"].Value);

            if (documentBookId == bookId)
            {
                bookNodes[i].ParentNode.RemoveChild(bookNodes[i]);
                break;
            }
        }

        Library.Books.RemoveAll(model => model.Id == bookId);

        StoreLibrary(LibraryFile);
    }

    public int InsertBook(string name, string author)
    {
        LoadLibrary(LibraryFile);

        var nextBookId = GenerateBookId();

        XmlNodeList bookNodes = _doc.SelectNodes("/Library/Book");

        var bookElement = _doc.CreateElement("Book");
        var idAttribute = _doc.CreateAttribute("id");
        idAttribute.InnerText = nextBookId.ToString();
        bookElement.Attributes.Append(idAttribute);

        var nameElement = _doc.CreateElement("Name");
        nameElement.InnerText = name;
        bookElement.AppendChild(nameElement);

        var authorElement = _doc.CreateElement("Author");
        authorElement.InnerText = author;
        bookElement.AppendChild(authorElement);

        bookNodes[bookNodes.Count - 1].ParentNode.AppendChild(bookElement);

        StoreLibrary(LibraryFile);
        return nextBookId;
    }

    public void UpdateBook(int bookId, string? name = null, string? author = null)
    {
        LoadLibrary(LibraryFile);

        XmlNodeList bookNodes = _doc.SelectNodes("/Library/Book");
        for (var i = 0; i < bookNodes.Count; i++)
        {
            XmlNode? bookNode = bookNodes[i];
            var documentBookId = int.Parse(bookNode.Attributes["id"].Value);
            if (documentBookId != bookId) continue;

            var selectSingleNode = bookNode.SelectSingleNode("Name");
            name ??= selectSingleNode.Value;
            selectSingleNode.InnerText = name;

            var authorNode = bookNode.SelectSingleNode("Author");
            author ??= authorNode.Value;
            authorNode.InnerText = author;
        }

        StoreLibrary(LibraryFile);
        UpdateLibraryModel();
    }

    public void BorrowBook(int bookId, string firstName, string lastName, DateTimeOffset from)
    {
        LoadLibrary(LibraryFile);
        
        XmlNodeList bookNodes = _doc.SelectNodes("/Library/Book");
        for (var i = 0; i < bookNodes.Count; i++)
        {
            XmlNode? bookNode = bookNodes[i];
            var documentBookId = int.Parse(bookNode.Attributes["id"].Value);
            if (documentBookId != bookId) continue;
            
            var selectSingleNode = bookNode.SelectSingleNode("Borrowed");

            if (selectSingleNode != null) return;
            
            
            var borrowedElement = _doc.CreateElement("Borrowed");
            var firstNameElement = _doc.CreateElement("FirstName");
            var lastNameElement = _doc.CreateElement("LastName");
            var fromElement = _doc.CreateElement("From");
            borrowedElement.AppendChild(firstNameElement);
            borrowedElement.AppendChild(lastNameElement);
            borrowedElement.AppendChild(fromElement);
            
            firstNameElement.InnerText = firstName;
            lastNameElement.InnerText = lastName;
            fromElement.InnerText = @from.ToString();
            
            
            bookNode?.AppendChild(borrowedElement);
            
            break;
        }
        
        StoreLibrary(LibraryFile);
        UpdateLibraryModel();
    }

    public void ReturnBook(int bookId)
    {
        LoadLibrary(LibraryFile);
        
        XmlNodeList bookNodes = _doc.SelectNodes("/Library/Book");
        for (var i = 0; i < bookNodes.Count; i++)
        {
            XmlNode? bookNode = bookNodes[i];
            var documentBookId = int.Parse(bookNode.Attributes["id"].Value);
            if (documentBookId != bookId) continue;
            
            var selectSingleNode = bookNode.SelectSingleNode("Borrowed");

            if (selectSingleNode == null) return;

            bookNode.RemoveChild(selectSingleNode);
            
            break;
        }
        
        StoreLibrary(LibraryFile);
        UpdateLibraryModel();
    }

    public BookModel? GetBook(int bookId)
    {
        return GetBooks().First(mod => mod.Id == bookId);
    }

    public List<BookModel> GetBooks()
    {
        LoadLibrary(LibraryFile);

        UpdateLibraryModel();

        return new List<BookModel>(Library.Books);
    }

    private void UpdateLibraryModel()
    {
        XmlNodeList bookNodes = _doc.SelectNodes("/Library/Book");

        LibraryModel libraryModel = new LibraryModel();

        foreach (XmlNode bookNode in bookNodes)
        {
            XmlAttribute idAttribute = bookNode.Attributes["id"];

            var selectSingleNode = bookNode.SelectSingleNode("Name");
            var authorNode = bookNode.SelectSingleNode("Author");
            var borrowedNode = bookNode.SelectSingleNode("Borrowed");

            BorrowModel? borrowedModel = null;

            if (borrowedNode != null)
            {
                var firstNameNode = borrowedNode.SelectSingleNode("FirstName");
                var lastNameNode = borrowedNode.SelectSingleNode("LastName");
                var from = borrowedNode.SelectSingleNode("From");

                DateTimeFormatInfo fmt = new CultureInfo("sk-SK").DateTimeFormat;

                borrowedModel = new BorrowModel()
                {
                    FirstName = firstNameNode.InnerText,
                    LastName = lastNameNode.InnerText,
                    From = DateTimeOffset.Parse(from.InnerText, fmt)
                };
            }

            var bookModel = new BookModel
            {
                Id = Int32.Parse(idAttribute.Value),
                Name = selectSingleNode.InnerText,
                Author = authorNode.InnerText,
                BorrowedBy = borrowedModel
            };

            libraryModel.Books.Add(bookModel);
        }

        _library = libraryModel;
    }
    
    private int GenerateBookId()
    {
        return Library.Books.Count + 1;
    }
}