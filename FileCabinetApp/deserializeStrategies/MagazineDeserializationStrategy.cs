using System.Text.Json;
using OOP_FileCabinetApp.interfaces;
using OOP_FileCabinetApp.types;

namespace OOP_FileCabinetApp.deserializeHelpers
{
    public class PatentDeserializationStrategy : IDocumentDeserializationStrategy
    {
        public bool CanHandle(string documentNumber) => documentNumber.StartsWith("patent_#");

        public IDocument Deserialize(string jsonData)
        {
            return JsonSerializer.Deserialize<Patent>(jsonData);
        }
    }
}
