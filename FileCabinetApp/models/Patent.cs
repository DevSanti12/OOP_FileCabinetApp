using System.Text.Json;
using OOP_FileCabinetApp.interfaces;

namespace OOP_FileCabinetApp.types
{
    public class Patent : IDocument
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string UniqueId { get; set; }
        public string DocumentNumber => $"patent_#{UniqueId}";

        public string GetCardInfo()
        {
            return $"Patent (Title: {Title}, Authors: {Authors}, Published: {DatePublished.ToShortDateString()}, Expires: {ExpirationDate.ToShortDateString()})";
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
