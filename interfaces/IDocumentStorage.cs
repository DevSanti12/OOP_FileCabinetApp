using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_FileCabinetApp.interfaces
{
    public interface IDocumentStorage
    {
        void SaveDocument(IDocument document);
        IDocument GetDocumentByNumber(string documentNumber);
    }
}
