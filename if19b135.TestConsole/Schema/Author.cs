using if19b135.OrmFramework.Attributes;
using if19b135.OrmFramework.LazyLoading;

namespace if19b135.TestConsole.Schema
{
    [Entity(TableName = "AUTHORS")]
    public class Author : Person
    {
        public double Salary { get; set; }

        [ForeignKey(ColumnName = "AuthorId")] 
        public LazyList<Book> Books { get;  set; }

        public Author()
        {
            Books = new LazyList<Book>(this, nameof(Books));
        }

        public override string ToString()
        {
   
            return $"{base.ToString()}, {nameof(Salary)}: {Salary}, {nameof(Books)}: {Books}";
        }
    }
}