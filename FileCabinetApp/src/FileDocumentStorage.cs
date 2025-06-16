using OOP_FileCabinetApp.deserializeHelpers;
using OOP_FileCabinetApp.interfaces;
using OOP_FileCabinetApp.src;

namespace OOP_FileCabinetApp.storage
{
    public class FileDocumentStorage : IDocumentStorage
    {
        public readonly string storageDirectory;
        private readonly DocumentDeserializationRegistry _deserializationRegistry;
        private readonly DocumentCache _cache; // Caching mechanism

        public FileDocumentStorage(string directory,
                                   DocumentDeserializationRegistry deserializationRegistry,
                                   DocumentCache cache)
        {
            storageDirectory = directory;
            if (!Directory.Exists(storageDirectory))
                Directory.CreateDirectory(storageDirectory);

            // Initialize the registry and register all default strategies.
            _deserializationRegistry = deserializationRegistry;
            // Initialize the registry and cache
            _deserializationRegistry = deserializationRegistry;
            _cache = cache;
        }

        public void SaveDocument(IDocument document)
        {
            string filePath = Path.Combine(storageDirectory, $"{document.DocumentNumber}.json");
            string jsonData = document.Serialize(); //Delegate serialization to the document itself
            File.WriteAllText(filePath, jsonData);

            // Clear cache for the document if it exists because it was updated
            _cache.Clear(document.DocumentNumber);
        }

        public IDocument GetDocumentByNumber(string documentNumber)
        {
            string filePath = Path.Combine(storageDirectory, $"{documentNumber}.json");
            if(!File.Exists(filePath))
                throw new FileNotFoundException($"Document {documentNumber} does not exist");

            string jsonData = File.ReadAllText(filePath);
            return _deserializationRegistry.DeserializeDocument(documentNumber, jsonData);
        }

        public string GetCardInfo(string documentNumber)
        {
            // Check if cached CardInfo exists
            if (_cache.TryGet(documentNumber, out string cachedCardInfo))
            {
                return cachedCardInfo; // Return cached CardInfo if valid
            }

            // Retrieve the document from storage if not cached
            IDocument document = GetDocumentByNumber(documentNumber);
            string cardInfo = document.GetCardInfo();

            // Cache the CardInfo
            _cache.Add(documentNumber, cardInfo, document.GetType());

            return cardInfo;
        }
    }
}
