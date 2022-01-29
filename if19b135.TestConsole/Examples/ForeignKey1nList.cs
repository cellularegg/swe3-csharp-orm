using System;
using if19b135.OrmFramework;
using if19b135.TestConsole.Schema;

namespace if19b135.TestConsole.Examples
{
    public static class ForeignKey1nList
    {
        public static void Example()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Foreign Key example (1:n) with list");
            Console.ForegroundColor = ConsoleColor.Black;
            Author a = Orm.Get<Author>(0);
            // Console.WriteLine($"Retrieving book");
            Console.WriteLine($"Retrieved Author {a}");
            Console.WriteLine("Books of author:");
            foreach (Book book in a.Books)
            {
                Console.WriteLine($"\t{book.ToString(false)}");
            }
        }
        
    }
}