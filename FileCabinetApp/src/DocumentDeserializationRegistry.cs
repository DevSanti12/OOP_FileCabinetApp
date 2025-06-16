using OOP_FileCabinetApp.deserializeHelpers;
using OOP_FileCabinetApp.interfaces;

namespace OOP_FileCabinetApp.src
{
    public class DocumentDeserializationRegistry
    {
        private readonly List<IDocumentDeserializationStrategy> strategies = new();

        public DocumentDeserializationRegistry()
        {
            strategies.Add(new BookDeserializationStrategy());
            strategies.Add(new LocalizedBookDeserializationStrategy());
            strategies.Add(new PatentDeserializationStrategy());
            strategies.Add(new MagazineDeserializationStrategy());
        }

        public void RegisterStrategy(IDocumentDeserializationStrategy strategy)
        {
            strategies.Add(strategy);
        }

        public IDocument DeserializeDocument(string documentNumber, string jsonData)
        {
            foreach (var strategy in strategies)
            {
                if (strategy.CanHandle(documentNumber))
                    return strategy.Deserialize(jsonData);
            }

            throw new InvalidOperationException("Unsupported document type");
        }
    }
}
