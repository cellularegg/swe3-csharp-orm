using System;
using if19b135.OrmFramework;
using if19b135.TestConsole.Schema;

namespace if19b135.TestConsole.Examples
{
    public static class ForeignKey1n
    {
        public static void Example()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Foreign Key example (1:n)");
            Console.ForegroundColor = ConsoleColor.Black;
            Author a = Orm.Get<Author>(0);
            Book b = new Book
            {
                Id = 0,
                Author = a,
                Title = "Herr der Ringe",
                ISBN = "123456789",
                Price = 10.5
            };
            Console.WriteLine($"Saving Book: {b}"); 
            Orm.Save(b);
            // Console.WriteLine($"Retrieving book");
            b = Orm.Get<Book>(0);
            Console.WriteLine($"Retrieved Book {b}");
        }
        
    }
}