using System;

namespace if19b135.OrmFramework.Attributes
{
    public class FieldAttribute : Attribute
    {
        public string ColumnName = null;
        public Type ColumnType = null;
        public bool IsNullable = true;
    }
}