using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OOP_FileCabinetApp.interfaces;

namespace OOP_FileCabinetApp.types
{
    public class Magazine : IDocument
    {
        public string Title { get; set; }
        public string Publisher { get; set; }
        public int ReleaseNumber {  get; set; }
        public DateTime DatePublished { get; set; }

        public string DocumentNumber => $"magazine_#{ReleaseNumber}";

        public string GetCardInfo()
        {
            return $"Magazine ( Title: {Title}, Publisher: {Publisher}, Release Number: {ReleaseNumber} Published: {DatePublished.ToShortDateString()} )";
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this); // Serialize itself
        }
    }
}
