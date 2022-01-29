using System.Collections.Generic;
using if19b135.OrmFramework.Attributes;

namespace if19b135.TestConsole.Schema
{
    [Entity(TableName = "STORES")]
    public class Store
    {
        [PrimaryKey] public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }

        [ForeignKey(AssignmentTable = "BOOKS_IN_STORE", ColumnName = "StoreId", RemoteColumnName = "BookId")]
        public List<Book> Books { get; private set; } = new List<Book>();

        public Store()
        {
        }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Address)}: {Address}, {nameof(Books)}: {Books}";
        }
    }
}