using OOP_FileCabinetApp.interfaces;
using OOP_FileCabinetApp.storage;
using OOP_FileCabinetApp.types;

class program
{
    static void Main()
    {
        string storageDirectory = "LibraryStorage";
        IDocumentStorage storage = new FileDocumentStorage(storageDirectory);
        DocumentSearcher searcher = new DocumentSearcher(storage);

        Console.WriteLine("Welcome to the File Cabinet System!");
        while(true)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Document");
            Console.WriteLine("2. Search Document by Number");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine() ?? throw new ArgumentNullException(); ;
            switch (choice)
            {
                case "1":
                    AddNewDocument(storage);
                    break;
                case "2":
                    SearchDocument(searcher);
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
    }
}

    static void SearchDocument(DocumentSearcher searcher)
    {
        Console.Write("Enter Document Number (<document_type>_#<document_number>):  ");
        string documentNumber = Console.ReadLine() ?? throw new ArgumentNullException(); ;
        IDocument document = searcher.SearchByNumber(documentNumber);
        if (document != null)
        {
            Console.WriteLine("Document Found:");
            Console.WriteLine(document.GetCardInfo());
        }
    }

    static void AddNewDocument(IDocumentStorage storage)
    {
        Console.WriteLine("Choose the document type: (1) Book, (2) Localized Book, (3) Patent ");
        string type = Console.ReadLine() ?? throw new ArgumentNullException();
        
        string book = "1";
        string localizedBook = "2";
        string patent = "3";

        IDocument document;
        if (type == book)
        {
            document = new Book
            {
                ISBN = Prompt("ISBN"),
                Title = Prompt("Title"),
                Authors = Prompt("Authors"),
                NumberOfPages = int.Parse(Prompt("Number of Pages")),
                Publisher = Prompt("Publisher"),
                DatePublished = DateTime.Parse(Prompt("Date Published"))
            };
        }
        else if (type == localizedBook)
        {
            document = new LocalizedBook
            {
                ISBN = Prompt("ISBN"),
                Title = Prompt("Title"),
                Authors = Prompt("Authors"),
                NumberOfPages = int.Parse(Prompt("Number of Pages")),
                OriginalPublisher = Prompt("Original Publisher"),
                CountryOfLocalization = Prompt("Country of Localization"),
                LocalPublisher = Prompt("Local Publisher"),
                DatePublished = DateTime.Parse(Prompt("Date Published"))
            };
        }
        else if (type == patent)
        {
            document = new Patent
            {
                Title = Prompt("Title"),
                Authors = Prompt("Authors"),
                DatePublished = DateTime.Parse(Prompt("Date Published")),
                ExpirationDate = DateTime.Parse(Prompt("Expiration Date")),
                UniqueId = Prompt("Unique ID")
            };
        }
        else
        {
            Console.WriteLine("Invalid document type.");
            return;
        }

        //Save doc
        storage.SaveDocument(document);
        Console.WriteLine("Document saved successfully.");
    }

    private static string Prompt(string field)
    {
        Console.Write($"Enter {field}: ");
        return Console.ReadLine() ?? throw new ArgumentNullException($"Not a valid {field}!"); ;
    }
}
