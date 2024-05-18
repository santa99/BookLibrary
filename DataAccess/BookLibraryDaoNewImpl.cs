using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Contracts;
using Contracts.Models;
using DataAccess.Configuration;
using Microsoft.Extensions.Options;

namespace DataAccess;

public class BookLibraryDaoNewImpl : IBookLibraryDao
{
    private readonly string _libraryXml;
    private readonly string _valueValidationSchema;
    private bool _schemaValidated;
    private readonly bool _isSchemaValidation;

    public BookLibraryDaoNewImpl(IOptions<BookLibraryDataSourceConfig> config)
    {
        var bookLibraryDataSourceConfig = config.Value;
        _libraryXml = bookLibraryDataSourceConfig.FilePath;
        _valueValidationSchema = bookLibraryDataSourceConfig.ValidationSchema;
        _isSchemaValidation = bookLibraryDataSourceConfig.IsSchemaValidation;
    }

    public int Create(BookModel bookModel)
    {
        var libraryModel = LoadDocument();

        if (bookModel == null)
        {
            throw new InvalidOperationException("Book model is null.");
        }

        var nextBookId = GetNextBookId(libraryModel.Books);
        var fromBook = BookModel.FromBook(bookModel);
        fromBook.Id = nextBookId;
        libraryModel.Books.Add(fromBook);

        Store(libraryModel);

        return nextBookId;
    }

    public BookModel? Read(int bookId)
    {
        if (bookId <= 0)
        {
            throw new InvalidOperationException("Invalid book id.");
        }

        var libraryModel = LoadDocument();

        return libraryModel.Books.FirstOrDefault(model => model.Id == bookId);
    }

    public void Update(BookModel bookModel)
    {
        if (bookModel == null)
        {
            throw new InvalidOperationException("Book model is null.");
        }
        
        if (bookModel.Id <= 0)
        {
            throw new InvalidOperationException("Invalid book id.");
        }

        var libraryModel = LoadDocument();

        var found = libraryModel.Books.FirstOrDefault(model => model.Id == bookModel.Id);

        if (found == null)
        {
            return;
        }
        
        libraryModel.Books.Remove(found);
        libraryModel.Books.Add(BookModel.FromBook(bookModel));
        Store(libraryModel);
    }

    public void Delete(int bookId)
    {
        if (bookId <= 0)
        {
            throw new InvalidOperationException("Invalid book id.");
        }

        var libraryModel = LoadDocument();

        var toRemove = libraryModel.Books.FirstOrDefault(model => model.Id == bookId);
        if (toRemove == null)
        {
            return;
        }
        libraryModel.Books.Remove(toRemove);
        Store(libraryModel);
    }

    public List<BookModel> GetBooks()
    {
        var libraryModel = LoadDocument();

        return new List<BookModel>(libraryModel.Books.OrderBy(model => model.Id));
    }
    
    private Library LoadDocument()
    {
        ValidateSchema();
        
        var xmlSerializer = new XmlSerializer(typeof(Library));
        TextReader? streamReader = null;
        Library? libraryModel = null;
        try
        {
            streamReader = new StreamReader(_libraryXml);
            var obj = xmlSerializer.Deserialize(streamReader);
            if (obj is Library library)
            {
                libraryModel = library;
            }
        }
        finally
        {
            streamReader?.Close();
        }

        if (libraryModel == null)
        {
            throw new InvalidOperationException("Can't read document.");
        }

        return libraryModel;
    }

    private void Store(Library library)
    {
        var xmlSerializer = new XmlSerializer(typeof(Library));
        StreamWriter streamWriter = null!;
        try
        {
            streamWriter = new StreamWriter(_libraryXml);
            xmlSerializer.Serialize(streamWriter, library);
        }
        finally
        {
            streamWriter.Close();
        }
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
        
        var xDocument = XDocument.Load(_libraryXml);
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

    private static int GetNextBookId(IEnumerable<BookModel> bookModels)
    {
        var nextBookId = bookModels.Select(model => model.Id).Prepend(-1).Max();
        nextBookId = nextBookId <= 0 ? 1 : nextBookId + 1;
        return nextBookId;
    }
}