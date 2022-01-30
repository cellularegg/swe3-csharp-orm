using System;

namespace if19b135.OrmFramework.Attributes
{
    /// <summary>
    /// Field attribute for a class. Annotation for Fields to specify the DB table column
    /// </summary>
    public class FieldAttribute : Attribute
    {
        /// <summary>
        /// Column Name in the DB
        /// </summary>
        public string ColumnName = null;
        /// <summary>
        /// Column Type of the DB column
        /// </summary>
        public Type ColumnType = null;
        /// <summary>
        /// Whether NOT NULL Constraint is set
        /// </summary>
        public bool IsNullable = true;
    }
}