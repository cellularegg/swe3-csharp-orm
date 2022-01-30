using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Attributes
{
    /// <summary>
    /// Marks a field as a Foreign key
    /// </summary>
    public class ForeignKeyAttribute : FieldAttribute
    {
        /// <summary>
        /// Specifies the table name where m:n relations are stores (only used for m:n relations)
        /// </summary>
        public string AssignmentTable = null;
        /// <summary>
        /// Specifies the other foreign key column name in the assignment table (only used for m:n relations)
        /// </summary>
        public string RemoteColumnName = null;
    }
}