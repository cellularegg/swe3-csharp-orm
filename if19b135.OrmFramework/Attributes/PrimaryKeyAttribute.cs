using System;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Attributes
{
    public class PrimaryKeyAttribute : FieldAttribute
    {
        public PrimaryKeyAttribute()
        {
            IsNullable = false;
        }
    }
}