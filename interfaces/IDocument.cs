using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_FileCabinetApp.interfaces
{
    //Write to an interface apporach
    public interface IDocument
    {
        string DocumentNumber { get; }
        string GetCardInfo();

        //leaving serialize here so Json.Serialize can be define for each type of document
        string Serialize(); 
    }
}
