using System;
using System.Data;
using if19b135.OrmFramework;

namespace if19b135.TestConsole.Examples
{
    public static class Reset
    {
        public static void Example()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Resetting DB");
            Console.ForegroundColor = ConsoleColor.Black;

            IDbCommand cmd = Orm.Connection.CreateCommand();
            
            cmd.CommandText = "DELETE FROM BOOKS";
            Console.WriteLine($"Executing: {cmd.CommandText}");
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            
            // cmd = Orm.Connection.CreateCommand();
            // cmd.CommandText = "DELETE FROM PERSONS";
            // Console.WriteLine($"Executing: {cmd.CommandText}");
            // cmd.ExecuteNonQuery();
            // cmd.Dispose();

            cmd = Orm.Connection.CreateCommand();
            cmd.CommandText = "DELETE FROM AUTHORS";
            Console.WriteLine($"Executing: {cmd.CommandText}");
            cmd.ExecuteNonQuery();
            cmd.Dispose();


        }
    }
}