using OOP_FileCabinetApp.interfaces;

namespace OOP_FileCabinetApp.storage
{
    public class DocumentSearcher
    {
        private readonly IDocumentStorage _storge;

        public DocumentSearcher(IDocumentStorage storage)
        {
            _storge = storage;
        }

        public IDocument SearchByNumber(string documentNumber)
        {
            try
            {
                return _storge.GetDocumentByNumber(documentNumber);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Document with number {documentNumber} not found.");
                return null;
            }
        }
    }
}
