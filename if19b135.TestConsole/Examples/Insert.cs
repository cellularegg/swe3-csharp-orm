using System;
using System.Collections.Generic;
using if19b135.TestConsole.Schema;
using if19b135.OrmFramework;

namespace if19b135.TestConsole.Examples
{
    public static class Insert
    {
        public static void Example()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Insert");
            Console.ForegroundColor = ConsoleColor.Black;
            Author a = new Author
            {
                Id = 0,
                FirstName = "Max",
                LastName = "Mustermann",
                Birthdate = DateTime.Now.AddYears(-30),
                SocialSecurityNumber = "max123",
                // Books = null,
                Salary = 1000
            };
            Console.WriteLine($"Inserting {a}");
            Orm.Save(a);
        }
    }
}