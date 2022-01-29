using System;
using System.Data;
using System.Data.SQLite;
using if19b135.OrmFramework;
using if19b135.TestConsole.Examples;
using Npgsql;

namespace if19b135.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Switched to SQLite because Docker stopped working suddenly
            // Orm.Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=mysecretpassword;Database=swe3");
            Orm.Connection = new SQLiteConnection("DataSource=if19b135_swe3.sqlite");
            Orm.Connection.Open();
            // Reset.Example();
            Insert.Example();
            GetAndModify.Example();
            ForeignKey1n.Example();
            ForeignKey1nList.Example();
            ForeignKeyMN.Example();
            // GetAndModify.Example();
            Console.WriteLine("Done!");
            Orm.Connection.Close();
            Orm.Connection.Dispose();
        }
    }
}