namespace OOP_FileCabinetApp.interfaces
{
    public interface IDocumentDeserializationStrategy
    {
        bool CanHandle(string documentNumber);
        IDocument Deserialize(string jsonData);
    }
}
