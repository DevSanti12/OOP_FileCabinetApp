using OOP_FileCabinetApp.deserializeHelpers;
using OOP_FileCabinetApp.settings;
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

        var cacheSettings = new Dictionary<Type, DocumentCacheSettings>
        {
            { typeof(Book), new DocumentCacheSettings { CacheDuration = TimeSpan.FromMinutes(30) } }, // Cache for 30 minutes
            { typeof(LocalizedBook), new DocumentCacheSettings { CacheDuration = TimeSpan.FromHours(1) } }, // Cache for 1 hour
            { typeof(Patent), new DocumentCacheSettings { DoNotCache = true } }, // No cache for Patents
            { typeof(Magazine), new DocumentCacheSettings { CacheDuration = TimeSpan.FromDays(1) } } // Cache for 1 day
        };

        // Initialize the cache
        var cache = new DocumentCache(cacheSettings);

        _storage = new FileDocumentStorage(_testDirectory, deserializationRegistry, cache);
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

    [Fact]
    public void GetCardInfo_ShouldReturnCachedCardInfo_WhenCacheIsValid()
    {
        // Arrange
        var cacheSettings = new Dictionary<Type, DocumentCacheSettings>
    {
        { typeof(Book), new DocumentCacheSettings { CacheDuration = TimeSpan.FromMinutes(30) } }
    };

        var cache = new DocumentCache(cacheSettings);

        var deserializationRegistry = new DocumentDeserializationRegistry();
        deserializationRegistry.RegisterStrategy(new BookDeserializationStrategy());

        var storage = new FileDocumentStorage("TestLibraryStorage", deserializationRegistry, cache);

        string documentNumber = "book_#12345";
        string jsonContent = @"
    {
        ""ISBN"": ""12345"",
        ""Title"": ""Sample Book"",
        ""Authors"": ""John Doe"",
        ""NumberOfPages"": 300,
        ""Publisher"": ""Test Publisher"",
        ""DatePublished"": ""2022-01-01T00:00:00"",
        ""DocumentNumber"": ""book_#12345""
    }";

        Directory.CreateDirectory("TestLibraryStorage"); // Create test storage directory
        File.WriteAllText(Path.Combine("TestLibraryStorage", $"{documentNumber}.json"), jsonContent);

        // Act
        string firstCardInfo = storage.GetCardInfo(documentNumber); // Retrieve and populate cache
        string secondCardInfo = storage.GetCardInfo(documentNumber); // Retrieve from cache

        // Assert
        Assert.Equal(firstCardInfo, secondCardInfo); // Cached card info should be identical
    }

    [Fact]
    public void GetCardInfo_ShouldReFetchCardInfo_WhenCacheExpires()
    {
        // Arrange
        var cacheSettings = new Dictionary<Type, DocumentCacheSettings>
    {
        { typeof(Book), new DocumentCacheSettings { CacheDuration = TimeSpan.FromSeconds(1) } }
    };

        var cache = new DocumentCache(cacheSettings);

        var deserializationRegistry = new DocumentDeserializationRegistry();
        deserializationRegistry.RegisterStrategy(new BookDeserializationStrategy());

        var storage = new FileDocumentStorage("TestLibraryStorage", deserializationRegistry, cache);

        string documentNumber = "book_#12345";
        string jsonContent = @"
    {
        ""ISBN"": ""12345"",
        ""Title"": ""Expiring Book"",
        ""Authors"": ""John Cache"",
        ""NumberOfPages"": 400,
        ""Publisher"": ""Cache Publisher"",
        ""DatePublished"": ""2023-01-01T00:00:00"",
        ""DocumentNumber"": ""book_#12345""
    }";

        Directory.CreateDirectory("TestLibraryStorage");
        File.WriteAllText(Path.Combine("TestLibraryStorage", $"{documentNumber}.json"), jsonContent);

        // Act
        string firstCardInfo = storage.GetCardInfo(documentNumber); // Cache populated
        Thread.Sleep(2000); // Wait for the cache to expire
        string secondCardInfo = storage.GetCardInfo(documentNumber); // Cache expired; re-fetch from storage

        // Assert
        Assert.Equal(firstCardInfo, secondCardInfo); // Ensure the result is the same after re-fetching
    }
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true); // Cleanup test files
        }
    }
}

