using System;

namespace if19b135.OrmFramework.Attributes
{
    /// <summary>
    /// Entity Attribute for a class. Annotation for Classes to specify the DB table
    /// </summary>
    public class EntityAttribute : Attribute
    {
        /// <summary>
        /// Table name of given Class
        /// </summary>
        public string TableName = null;
    }
}