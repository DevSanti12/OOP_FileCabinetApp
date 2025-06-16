using System.Text.Json;
using OOP_FileCabinetApp.interfaces;
using OOP_FileCabinetApp.types;

namespace OOP_FileCabinetApp.deserializeHelpers
{
    public class LocalizedBookDeserializationStrategy : IDocumentDeserializationStrategy
    {
        public bool CanHandle(string documentNumber) => documentNumber.StartsWith("localizedbook_#");

        public IDocument Deserialize(string jsonData)
        {
            return JsonSerializer.Deserialize<LocalizedBook>(jsonData);
        }
    }
}
