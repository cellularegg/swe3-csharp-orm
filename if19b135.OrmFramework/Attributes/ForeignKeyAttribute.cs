using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Attributes
{
    public class ForeignKeyAttribute : FieldAttribute
    {
        public string AssignmentTable = null;
        public string RemoteColumnName = null;
    }
}