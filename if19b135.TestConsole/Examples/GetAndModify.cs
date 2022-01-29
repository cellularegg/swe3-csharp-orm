using System;
using if19b135.OrmFramework;
using if19b135.TestConsole.Schema;

namespace if19b135.TestConsole.Examples
{
    public static class GetAndModify
    {
        public static void Example()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Get and modify Object");
            Console.ForegroundColor = ConsoleColor.Black;

            Author a = OrmFramework.Orm.Get<Author>(0);

            Console.WriteLine($"Retrieved Author: {a}");

            a.SocialSecurityNumber = "Mustermann1";
            a.Salary = 1234;

            Console.WriteLine($"Saving Author: {a}");

            Orm.Save(a);
        }
    }
}