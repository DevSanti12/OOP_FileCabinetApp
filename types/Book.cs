using System.Text.Json;
using OOP_FileCabinetApp.interfaces;

namespace OOP_FileCabinetApp.types
{
    public class Book : IDocument
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public int NumberOfPages { get; set; }
        public string Publisher { get; set; }
        public DateTime DatePublished { get; set; }

        public string DocumentNumber => $"book_#{ISBN}";

        public string GetCardInfo()
        {
            return $"Book (ISBN: {ISBN}, Title: {Title}, Authors: {Authors}, Publisher: {Publisher}, Published: {DatePublished.ToShortDateString()}";
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this); // Serialize itself
        }
    }
}
