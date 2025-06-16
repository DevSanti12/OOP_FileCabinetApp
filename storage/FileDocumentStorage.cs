using System.Text.Json;
using OOP_FileCabinetApp.interfaces;
using OOP_FileCabinetApp.types;

namespace OOP_FileCabinetApp.storage
{
    public class FileDocumentStorage : IDocumentStorage
    {
        public readonly string storageDirectory;

        public FileDocumentStorage(string directory) 
        {
            storageDirectory = directory;
            if (!Directory.Exists(storageDirectory))
            {
                Directory.CreateDirectory(storageDirectory);
            }
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
            return DeserializedDocument(documentNumber, jsonData);
        }

        private IDocument DeserializedDocument(string documentNumber, string jsonData)
        {
            if(documentNumber.StartsWith("book_#"))
                return JsonSerializer.Deserialize<Book>(jsonData);
            if (documentNumber.StartsWith("localizedbook_#"))
                return JsonSerializer.Deserialize<LocalizedBook>(jsonData);
            if (documentNumber.StartsWith("patent_#"))
                return JsonSerializer.Deserialize<Patent>(jsonData);
            if (documentNumber.StartsWith("magazine_#"))
                return JsonSerializer.Deserialize<Magazine>(jsonData);

            throw new InvalidOperationException("Unsupported document type");

        }
    }
}
