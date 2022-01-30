using System;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Attributes
{
    /// <summary>
    /// Marks a primary key. Sets IsNullable automatically to false
    /// </summary>
    public class PrimaryKeyAttribute : FieldAttribute
    {
        public PrimaryKeyAttribute()
        {
            IsNullable = false;
        }
    }
}