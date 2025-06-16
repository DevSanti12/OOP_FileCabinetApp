using OOP_FileCabinetApp.deserializeHelpers;
using OOP_FileCabinetApp.interfaces;
using OOP_FileCabinetApp.src;

namespace OOP_FileCabinetApp.storage
{
    public class FileDocumentStorage : IDocumentStorage
    {
        public readonly string storageDirectory;
        private readonly DocumentDeserializationRegistry _deserializationRegistry;

        public FileDocumentStorage(string directory, DocumentDeserializationRegistry deserializationRegistry) 
        {
            storageDirectory = directory;
            if (!Directory.Exists(storageDirectory))
                Directory.CreateDirectory(storageDirectory);

            // Initialize the registry and register all default strategies.
            _deserializationRegistry = deserializationRegistry;
        }

        public void SaveDocument(IDocument document)
        {
            string filePath = Path.Combine(storageDirectory, $"{document.DocumentNumber}.json");
            string jsonData = document.Serialize(); //Delegate serialization to the document itself
            File.WriteAllText(filePath, jsonData);
        }

        public IDocument GetDocumentByNumber(string documentNumber)
        {
            string filePath = Path.Combine(storageDirectory, $"{documentNumber}.json");
            if(!File.Exists(filePath))
                throw new FileNotFoundException($"Document {documentNumber} does not exist");

            string jsonData = File.ReadAllText(filePath);
            return _deserializationRegistry.DeserializeDocument(documentNumber, jsonData);
        }
    }
}
