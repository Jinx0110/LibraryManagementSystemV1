using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Data;
using System.Transactions;
using System.Runtime.ExceptionServices;

class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public DateOnly PublicationDate { get; set; }
    
}

class Program
{
   public List<Book> books = new List<Book>();
   public String longestBookTitle = string.Empty;
   public const string cancel = "X";
   public const string path = "C:/Visual Studio IDE/Side Projects/LibraryManagementSystem/LibraryBooks.txt";

    //TABLE VARIABLES
    static int TableWidth = 110;
    static ConsoleColor HeaderColor = ConsoleColor.Cyan;
    static ConsoleColor DefaultColor = ConsoleColor.White;
    static ConsoleColor HighlightColor = ConsoleColor.Yellow;
    static ConsoleColor NameFontColor = ConsoleColor.DarkGreen;
    static ConsoleColor Pink = ConsoleColor.Magenta;
    static ConsoleColor DarkPink = ConsoleColor.DarkMagenta;


    static void Main(string[] args)
    {
        Program program = new Program();
        //string path = "C:/Visual Studio IDE/Side Projects/LibraryManagementSystem/LibraryBooks.txt";
        program.ReadFromTxt();
        program.Menu();
        //program.DisplayBooks();

    }

    public void Menu()
    {
        const string construction = "ERROR: SORRY THIS OPTION IS CURRENTLY UNAVAIBLE";
        Console.WriteLine("MENU");
        Console.WriteLine("1. Display All Book Information");
        Console.WriteLine("2. Add Book");
        Console.WriteLine("3. Remove Book");
        Console.WriteLine("4. Edit Book Information");
        Console.WriteLine("5. Exit");

        try
        {
            int menuChoice = Convert.ToInt32(Console.ReadLine());
            
            switch (menuChoice)
            {
                case 1: DisplayBooks();
                        Menu();
                    break;
                case 2:AddBook(); 
                       Menu();
                    break;
                case 3: RemoveBook();
                        Menu();
                    break;
                case 4:EditBookDetails();
                        Menu();
                    break;
                case 5: Environment.Exit(0);
                    break;
                default: Console.WriteLine("ERROR: INCORRECT INPUT");
                        Menu();
                    break;
            }
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"ERROR : {ex.Message}\n");
            Menu();
        }
        

    }

    public void AddBook()
    {
        //GET RID OF THE NEED TO PRESS A KEY OR ENTER TO GET THE LINE PROMPTING FOR THE BOOKS INFORMATION, 
        //AFTER THE FOLLOWING LINE IS DISPLAYED, THE PROGRAM SHOULD IMMEDIATELY PROMPT THE USER FOR THE BOOKS DETAILS.
        Console.WriteLine("ENTER THE FOLLOWING BOOK INFORMATION\nTYPE 'x' TO EXIT");
        string addBookName = string.Empty;
        string addBookAuthor = string.Empty;
        string addBookGenre = string.Empty;
        string addPubDate = string.Empty;

        while (true)
        {
            Console.Write("Enter Book Name: ");
            addBookName = Console.ReadLine();
            if (addBookName.ToUpper() == cancel)
            {
                break;
                Menu();
            }
            Console.Write("Enter Author: ");
            addBookAuthor = Console.ReadLine();
            Console.Write("Enter Genre: ");
            addBookGenre = Console.ReadLine();
            Console.Write("Enter Publication Date(YYYY/MM/DD): ");
            addPubDate = Console.ReadLine();
            Console.Write("ADD BOOK(Y(yes) or N(no)): ");
            string add = Console.ReadLine();
            if (add.ToUpper() == "Y")
            {
                if (DateOnly.TryParse(addPubDate, out DateOnly publicationDate))
                {
                books.Add(new Book
                    {
                    Title = addBookName,
                    Author = addBookAuthor,
                    Genre = addBookGenre,
                    PublicationDate = publicationDate
                    });
                }
                //SAVE TO TXT FILE
                try
                {

                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        foreach (var book in books)
                        {
                            writer.WriteLine($"{book.Title},{book.Author},{book.Genre},{book.PublicationDate:yyyy-MM-dd}");
                        }
                    }
                    Console.WriteLine("Book saved successfully\n");
                    Menu();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }

            }
            else if(add.ToUpper() == "N")
            {
                Console.WriteLine("Book Addition Canceled");
                Menu();
            }
            else
            {
                Console.WriteLine("ERROR: Invalid Input");
                AddBook();
            }   
        }

    }

    public void RemoveBook()
    {
        Console.WriteLine("\nCurrent Books:");
        DisplayBooks();
        Console.WriteLine("TYPE 'X' TO RETURN TO MENU");
        try
        {
        Console.Write("\nEnter book title to delete: ");
        string bookTitle = Console.ReadLine();
            if (bookTitle.ToUpper() == cancel)
            {
                Menu();
            }
            else
            {
            //CHECK TO SEE IF ENETRED TITLE IS IN THE LIBRARY
            Book removeBook = books.FirstOrDefault(book => book.Title.Equals(bookTitle,StringComparison.CurrentCultureIgnoreCase));    
            if (removeBook != null)
            {
                try
                {
                 Console.WriteLine("Book Found\nProceed to remove book? (Type 'Y' to remove or 'N' to cancel):");
                 string cancel;
                    while (true)
                    {
                      cancel = Console.ReadLine();
                        if (cancel.ToUpper() == "Y")
                        {
                            books.Remove(removeBook);
                            Console.WriteLine("Book Removed Successfully\n");
                            RemoveBookFromFile(bookTitle); //REMOVE BOOK FROM TXT FILE
                            Menu();
                            break; //EXIT LOOP
                        }
                        else if (cancel.ToUpper() == "N")
                        {
                            Console.WriteLine("Remove Book Cancelled\n");
                            RemoveBook();
                            break; //EXIT LOOP
                        }
                        else
                        {
                            Console.WriteLine("Invalid Input.Enter 'Y' or 'N'");
                        }
                     }
                }
                 catch(FormatException fx)
                  {
                    Console.WriteLine($"ERROR: {fx.Message}");
                    RemoveBook();
                  }
                 catch (Exception ex)
                  {
                    Console.WriteLine($"ERROR: {ex.Message}");
                  }  
            }
            else
            {
                Console.WriteLine("BOOK DOES NOT EXIST");
                RemoveBook();
            }
            }
        }
        catch (FormatException fx)
        {
            Console.WriteLine($"Error: {fx.Message}\n");
            RemoveBook();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\n");
            RemoveBook();
        }
        
        
    }

    public void RemoveBookFromFile(string bookTitle)
    {
        List<string> fileLines = File.ReadAllLines(path).ToList();

        string removeBook = fileLines.FirstOrDefault(line => line.StartsWith(bookTitle, StringComparison.OrdinalIgnoreCase));
        if (removeBook != null)
        {
            fileLines.Remove(removeBook);
            //UPDATE LIST BACK TO FILE
            File.WriteAllLines(path, fileLines);   
        }


    }

    public void EditBookDetails()
    {
        Console.WriteLine("\nCurrent Books in Library: ");
        DisplayBooks();
        Console.Write("\nType 'X' to exit\nEnter Book Title to edit: ");
        string editThisBook;

        while (true)
        {
            editThisBook = Console.ReadLine();
        
        if (editThisBook.ToUpper() == cancel)
        {
            Menu();
        }
        else
        {
            try
            {
            Book editBook = books.FirstOrDefault(book => book.Title.Equals(editThisBook,StringComparison.OrdinalIgnoreCase));
            if (editBook != null)
            {
                Console.WriteLine($"\nCurrent Book details:\nTitle: {editBook.Title}\nAuthor: {editBook.Author}\nGenre: {editBook.Genre}\nPublication Date: {editBook.PublicationDate:yyyy-MM-DD}");
   
                try
                {
                        Console.WriteLine("\nEdit the following information:\n1. Title\n2. Author\n3. Genre\n4. Publication Date\n5. None");
                        int editThisDetail = Convert.ToInt32(Console.ReadLine());
                        
                        switch (editThisDetail)
                        {
                            case 1: Console.Write("Enter new book title: ");
                                    string newTitle = Console.ReadLine();
                                    Console.WriteLine($"Confirm to change book title '{editBook.Title}'  to '{newTitle}' (yes 'Y' OR no 'N')");
                                    string choice;
                                    
                                    while (true)
                                    {
                                        try
                                        {
                                        choice = Console.ReadLine();
                                        if (choice.ToUpper() == "Y")
                                        {
                                            editBook.Title = newTitle;
                                            break;
                                        }
                                        else if(choice.ToUpper() == "N")
                                        {
                                            Console.WriteLine("Book Edit Cancelled");
                                            EditBookDetails();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid Input. Type either 'Y' or 'N'");
                                            }
                                        }
                                        catch (Exception fx)
                                        {
                                            Console.WriteLine($"Error: {fx.Message}");
                                        }
                                    }
                                    
                                break;
                            case 2: Console.Write("Enter new book author: ");
                                    string newAuthor = Console.ReadLine();
                                    Console.WriteLine($"Confirm to change book title '{editBook.Author}'  to '{newAuthor}' (yes 'Y' OR no 'N')");
                                    string choice2;

                                    while (true)
                                    {
                                        try
                                        {
                                            choice2 = Console.ReadLine();
                                            if (choice2.ToUpper() == "Y")
                                            {
                                                editBook.Author = newAuthor;
                                                break;
                                            }
                                            else if (choice2.ToUpper() == "N")
                                            {
                                                Console.WriteLine("Book Edit Cancelled");
                                                EditBookDetails();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid Input. Type either 'Y' or 'N'");
                                            }
                                        }
                                        catch (Exception fx)
                                        {
                                            Console.WriteLine($"Error: {fx.Message}");
                                        }
                                    }
                                break;
                            case 3: Console.Write("Enter new book genre: ");
                                    string newGenre = Console.ReadLine();
                                    Console.WriteLine($"Confirm to change book title '{editBook.Genre}'  to '{newGenre}' (yes 'Y' OR no 'N')");
                                    string choice3;

                                    while (true)
                                    {
                                        try
                                        {
                                            choice3 = Console.ReadLine();
                                            if (choice3.ToUpper() == "Y")
                                            {
                                                editBook.Author = newGenre;
                                                break;
                                            }
                                            else if (choice3.ToUpper() == "N")
                                            {
                                                Console.WriteLine("Book Edit Cancelled");
                                                EditBookDetails();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid Input. Type either 'Y' or 'N'");
                                            }
                                        }
                                        catch (Exception fx)
                                        {
                                            Console.WriteLine($"Error: {fx.Message}");
                                        }
                                    }
                                    break;    
                            case 4: 
                                string newPubDateInfo;
                                DateOnly publicationDate;

                                while (true)
                                {
                                    Console.Write("Enter new book publication date(YYYY/MM/DD):");
                                    newPubDateInfo = Console.ReadLine();
                                      
                                        if (Regex.IsMatch(newPubDateInfo, @"^\d{4}/\d{2}/\d{2}$") && DateOnly.TryParse(newPubDateInfo, out publicationDate))
                                        {
                                            Console.WriteLine($"Confirm to change book publication date '{editBook.PublicationDate}'  to '{newPubDateInfo}' (yes 'Y' OR no 'N')");
                                            string choice4;
                                            while (true)
                                            {
                                                try
                                                {
                                                    choice4 = Console.ReadLine();
                                                    if (choice4.ToUpper() == "Y")
                                                    {
                                                        editBook.PublicationDate = publicationDate;
                                                        SaveEditedBookToFile();
                                                        Console.WriteLine("Book Information Edited Successfully");
                                                        Menu();
                                                        break;
                                                        
                                                    }
                                                    else if (choice4.ToUpper() == "N")
                                                    {
                                                        Console.WriteLine("Book Edit Cancelled");
                                                        break;
                                                        EditBookDetails(); 
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Invalid Input. Type either 'Y' or 'N'");
                                                    }
                                                }
                                                catch (Exception fx)
                                                {
                                                    Console.WriteLine($"Error: {fx.Message}");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Error: Invalid date format. Please enter the date in YYYY/MM/DD format.");
                                        } 
                                }
                                break;

                            case 5: Console.WriteLine("Edit Book Information Cancelled");
                                Menu();
                                return;
                                
                            default:Console.WriteLine("Error: Invalid Input");
                                return;
                        }

                        SaveEditedBookToFile();

                        Console.WriteLine("Book Edited Successfully");
                        EditBookDetails();
                }
                catch (FormatException fx)
                {
                    Console.WriteLine($"ERROR: {fx.Message}");
                }
                }
            else
            {
               Console.WriteLine("Book not found");
               Console.Write("Enter Book title to edit: ");
            }
            }
            catch (FormatException fx)
            {
                Console.WriteLine($"Error: {fx.Message}");
                Menu();
            }
           
        
        }
        }
            
    }

    public void SaveEditedBookToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var book in books)
                {
                    writer.WriteLine($"{book.Title},{book.Author},{book.Genre},{book.PublicationDate:yyyy-MM-dd}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: { ex.Message}");
        }
        
    }

    public void ReadFromTxt()
    {
        try
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        
                        string[] bookInfo = line.Split(',');
                        if (bookInfo.Length > 0)
                        {
                            string title = bookInfo[0].Trim();
                            string author = bookInfo[1].Trim();
                            string genre = bookInfo[2].Trim();
                            DateOnly publicationDate;

                            if (title.Length > longestBookTitle.Length)
                            {
                                longestBookTitle = title;
                            }

                            if (DateOnly.TryParse(bookInfo[3].Trim(), out publicationDate))
                            {

                                books.Add(new Book
                                {
                                    Title = title,
                                    Author = author,
                                    PublicationDate = publicationDate,
                                    Genre = genre
                                }); ;;
                            }

             
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR: FILE NOT FOUND");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }
    }

    public void DisplayBooks() //Drawing table to display neatly and more readable
    {
        if (books.Count == 0)
        {
            Console.WriteLine("-- NO BOOKS FOUND --");
        }

        
        PrintHeader();
        foreach (var book in books)
        {
            PrintRow(
                book.Title,
                book.Author,
                book.Genre,
                book.PublicationDate.ToString()
                );
        }
        /*foreach (var book in books)
        {
            Console.WriteLine($"{book.Title} {book.Author} {book.Genre} {book.PublicationDate}");
            Console.WriteLine(new string('-', 20)); // Separator for readability
        }*/
    }


    static void PrintHeader()
    {
        Console.ForegroundColor = HeaderColor;
        Console.WriteLine(new string('-', TableWidth));

        PrintRow("Book Title", "Author", "Genre", "Publication Date");
        Console.WriteLine(new string('-',TableWidth));
        Console.ResetColor();
    }

    static void PrintRow(params string[] columns)
    {
        int[] columnWidths = { 27, 30, 30, 18 };
        string row = "|";

        for (int i = 0; i < columns.Length; i++)
        {
            switch (i)
            {
                case 1: Console.ForegroundColor = NameFontColor;
                    break;
                case 2: Console.ForegroundColor = Pink;
                    break;
                case 3: Console.ForegroundColor = HighlightColor;
                    break;
                case 4: Console.ForegroundColor = DarkPink;
                    break;
                default: Console.ForegroundColor = DefaultColor;
                    break;
            }
            row += AlignColumn(columns[i], columnWidths[i]) + "|";
 

        }
        Console.WriteLine(row);
        Console.ResetColor();
    }

    static string AlignColumn(string text, int width)
    {
        if (string.IsNullOrEmpty(text))
            return new string(' ', width);
        text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;
        return text.PadRight(width);
    }

}