using System.Text.Json;
using OOP_FileCabinetApp.interfaces;

namespace OOP_FileCabinetApp.types
{
    public class LocalizedBook : IDocument
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public int NumberOfPages { get; set; }
        public string OriginalPublisher { get; set; }
        public string CountryOfLocalization { get; set; }
        public string LocalPublisher { get; set; }
        public DateTime DatePublished { get; set; }
        public string DocumentNumber => $"localizedbook_#{ISBN}";

        public string GetCardInfo()
        {
            return $"Localized Book (ISBN: {ISBN}, Title: {Title}, Authors: {Authors}, Local Publisher: {LocalPublisher}, Country: {CountryOfLocalization}, Published: {DatePublished.ToShortDateString()}";
        }
        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
