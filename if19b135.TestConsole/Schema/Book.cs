using if19b135.OrmFramework.Attributes;
using if19b135.OrmFramework.LazyLoading;

namespace if19b135.TestConsole.Schema
{
    [Entity(TableName = "BOOKS")]
    public class Book
    {
        [ForeignKey(ColumnName = "AuthorId")]
        private LazyObject<Author> _Author { get; set; } = new();

        [PrimaryKey] public int Id { get; set; }

        public string Title { get; set; }

        public string ISBN { get; set; }

        public double Price { get; set; }

        [Ignore]
        public Author Author
        {
            get { return _Author.Value; }
            set { _Author.Value = value; }
        }

        public Book()
        {
        }

        public string ToString(bool withAuthor)
        {
            return withAuthor
                ? this.ToString()
                : $"{nameof(Id)}: {Id}, {nameof(Title)}: {Title}, {nameof(ISBN)}: {ISBN}, {nameof(Price)}: {Price}";
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Title)}: {Title}, {nameof(ISBN)}: {ISBN}, {nameof(Price)}: {Price}, " +
                   $"{nameof(Author)}: {Author}";
        }
    }
}