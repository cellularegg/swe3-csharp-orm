using System;
using System.Collections.Generic;
using if19b135.OrmFramework;
using if19b135.TestConsole.Schema;

namespace if19b135.TestConsole.Examples
{
    public static class ForeignKeyMN
    {
        public static void Example()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Foreign Key example (m:n).");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Adding store with books");
            Store s = new Store
            {
                Id = 0,
                Address = "Musterstrasse 1",
                Name = "Mega book store"
            };
            Book b1 = Orm.Get<Book>(0);
            s.Books.Add(b1);
            Author a = new Author
            {
                Id = 1,
                Birthdate = DateTime.Now.AddYears(-25),
                FirstName = "Eva",
                LastName = "Musterfrau",
                Salary = 10000,
                SocialSecurityNumber = "EvaMusterfrau123"
            };
            Book b2 = new Book
            {
                Id = 1,
                Title = "harry potter",
                Author = a,
                ISBN = "111111",
                Price = 29.95                
                
            };
            Orm.Save(b2);
            Orm.Save(a);
            s.Books.Add(b2);
            
            Orm.Save(s);

            s = Orm.Get<Store>(0);
            Console.WriteLine(s);
            foreach (Book b in s.Books)
            {
                Console.WriteLine($"\t{b.ToString(false)}");
            }

        }
    }
}