using OOP_FileCabinetApp.deserializeHelpers;
using OOP_FileCabinetApp.src;
using OOP_FileCabinetApp.storage;
using OOP_FileCabinetApp.types;

namespace OOP_FileCabinetAppUnitTests;
public class FileDocumentStorageTests : IDisposable
{
    private readonly string _testDirectory = "TestLibraryStorage";
    private readonly FileDocumentStorage _storage;

    public FileDocumentStorageTests()
    {
        // Create a test directory
        if (!Directory.Exists(_testDirectory))
        {
            Directory.CreateDirectory(_testDirectory);
        }

        // Initialize deserialization registry and storage
        var deserializationRegistry = new DocumentDeserializationRegistry();
        deserializationRegistry.RegisterStrategy(new BookDeserializationStrategy());
        deserializationRegistry.RegisterStrategy(new LocalizedBookDeserializationStrategy());
        deserializationRegistry.RegisterStrategy(new PatentDeserializationStrategy());
        deserializationRegistry.RegisterStrategy(new MagazineDeserializationStrategy());

        _storage = new FileDocumentStorage(_testDirectory, deserializationRegistry);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldReturnBookObject_WhenBookExists()
    {
        // Arrange
        string documentNumber = "book_#12345";
        string jsonContent = @"
        {
            ""ISBN"": ""12345"",
            ""Title"": ""Test Book"",
            ""Authors"": ""John Doe"",
            ""NumberOfPages"": 300,
            ""Publisher"": ""Test Publisher"",
            ""DatePublished"": ""2022-01-01T00:00:00"",
            ""DocumentNumber"": ""book_#12345""
        }";

        File.WriteAllText(Path.Combine(_testDirectory, $"{documentNumber}.json"), jsonContent);

        // Act
        var document = _storage.GetDocumentByNumber(documentNumber);

        // Assert
        Assert.NotNull(document);
        Assert.IsType<Book>(document);
        var book = (Book)document;
        Assert.Equal("12345", book.ISBN);
        Assert.Equal("Test Book", book.Title);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldReturnLocalizedBookObject_WhenLocalizedBookExists()
    {
        // Arrange
        string documentNumber = "localizedbook_#56789";
        string jsonContent = @"
        {
            ""ISBN"": ""56789"",
            ""Title"": ""Localized Programming"",
            ""Authors"": ""Jane Smith"",
            ""NumberOfPages"": 400,
            ""OriginalPublisher"": ""Original Publishing"",
            ""CountryOfLocalization"": ""UK"",
            ""LocalPublisher"": ""Localized Inc."",
            ""DatePublished"": ""2023-05-01T00:00:00"",
            ""DocumentNumber"": ""localizedbook_#56789""
        }";

        File.WriteAllText(Path.Combine(_testDirectory, $"{documentNumber}.json"), jsonContent);

        // Act
        var document = _storage.GetDocumentByNumber(documentNumber);

        // Assert
        Assert.NotNull(document);
        Assert.IsType<LocalizedBook>(document);
        var localizedBook = (LocalizedBook)document;
        Assert.Equal("Localized Programming", localizedBook.Title);
        Assert.Equal("Jane Smith", localizedBook.Authors);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldReturnPatentObject_WhenPatentExists()
    {
        // Arrange
        string documentNumber = "patent_#111";
        string jsonContent = @"
        {
            ""Title"": ""Innovative Patent"",
            ""Authors"": ""John Innovator"",
            ""DatePublished"": ""2015-01-01T00:00:00"",
            ""ExpirationDate"": ""2030-01-01T00:00:00"",
            ""UniqueId"": ""PAT123"",
            ""DocumentNumber"": ""patent_#111""
        }";

        File.WriteAllText(Path.Combine(_testDirectory, $"{documentNumber}.json"), jsonContent);

        // Act
        var document = _storage.GetDocumentByNumber(documentNumber);

        // Assert
        Assert.NotNull(document);
        Assert.IsType<Patent>(document);
        var patent = (Patent)document;
        Assert.Equal("PAT123", patent.UniqueId);
        Assert.Equal("Innovative Patent", patent.Title);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldReturnMagazineObject_WhenMagazineExists()
    {
        // Arrange
        string documentNumber = "magazine_#999";
        string jsonContent = @"
        {
            ""Title"": ""Tech Innovations"",
            ""Publisher"": ""TechMedia"",
            ""ReleaseNumber"": 10,
            ""PublishDate"": ""2023-06-01T00:00:00"",
            ""DocumentNumber"": ""magazine_#999""
        }";

        File.WriteAllText(Path.Combine(_testDirectory, $"{documentNumber}.json"), jsonContent);

        // Act
        var document = _storage.GetDocumentByNumber(documentNumber);

        // Assert
        Assert.NotNull(document);
        Assert.IsType<Magazine>(document);
        var magazine = (Magazine)document;
        Assert.Equal("Tech Innovations", magazine.Title);
        Assert.Equal(10, magazine.ReleaseNumber);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        string documentNumber = "book_#999999";

        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() =>
            _storage.GetDocumentByNumber(documentNumber));

        Assert.NotNull(exception);
        Assert.Equal($"Document {documentNumber} does not exist", exception.Message);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldThrowInvalidOperationException_WhenPrefixIsUnsupported()
    {
        // Arrange
        string documentNumber = "unsupportedtype_#999";
        string jsonContent = @"
    {
        ""Title"": ""Unsupported Document"",
        ""DocumentNumber"": ""unsupportedtype_#999""
    }";

        File.WriteAllText(Path.Combine(_testDirectory, $"{documentNumber}.json"), jsonContent);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _storage.GetDocumentByNumber(documentNumber));

        Assert.NotNull(exception);
        Assert.Equal("Unsupported document type", exception.Message);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldThrowJsonException_WhenJsonIsMalformed()
    {
        // Arrange
        string documentNumber = "book_#123";
        string malformedJson = "{ invalid json }";

        File.WriteAllText(Path.Combine(_testDirectory, $"{documentNumber}.json"), malformedJson);

        // Act & Assert
        var exception = Assert.Throws<System.Text.Json.JsonException>(() =>
            _storage.GetDocumentByNumber(documentNumber));

        Assert.NotNull(exception);
    }

    [Fact]
    public void GetDocumentByNumber_ShouldThrowJsonException_WhenJsonIsEmpty()
    {
        // Arrange
        string documentNumber = "book_#456";
        string emptyJson = "";

        File.WriteAllText(Path.Combine(_testDirectory, $"{documentNumber}.json"), emptyJson);

        // Act & Assert
        var exception = Assert.Throws<System.Text.Json.JsonException>(() =>
            _storage.GetDocumentByNumber(documentNumber));

        Assert.NotNull(exception);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true); // Cleanup test files
        }
    }
}

