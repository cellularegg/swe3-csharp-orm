using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using if19b135.OrmFramework.Attributes;

namespace if19b135.OrmFramework.Metadata
{
    /// <summary>
    /// Represents a class / table
    /// </summary>
    internal class Entity
    {
        /// <summary>
        /// Constructor creates a new instance of Entity
        /// </summary>
        /// <param name="t">Type of the entity</param>
        public Entity(Type t)
        {
            EntityAttribute entityAttribute = (EntityAttribute)t.GetCustomAttribute(typeof(EntityAttribute));
            // if (entityAttribute == null || string.IsNullOrWhiteSpace(entityAttribute.TableName))
            // {
            //     
            // }
            TableName = string.IsNullOrWhiteSpace(entityAttribute?.TableName)
                ? t.Name.ToUpper()
                : entityAttribute.TableName;
            Member = t;

            List<Field> fields = new List<Field>();

            foreach (PropertyInfo propertyInfo in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic |
                                                                  BindingFlags.Instance))
            {
                if (((IgnoreAttribute)propertyInfo.GetCustomAttribute(typeof(IgnoreAttribute))) != null)
                {
                    continue;
                }

                Field field = new Field(this);
                FieldAttribute fieldAttribute =
                    (FieldAttribute)propertyInfo.GetCustomAttribute(typeof(FieldAttribute));

                if (fieldAttribute != null)
                {
                    if (fieldAttribute is PrimaryKeyAttribute)
                    {
                        PrimaryKey = field;
                        field.IsPrimaryKey = true;
                    }

                    field.ColumnName = fieldAttribute.ColumnName ?? propertyInfo.Name;
                    field.ColumnType = fieldAttribute.ColumnType ?? propertyInfo.PropertyType;
                    field.IsNullable = fieldAttribute.IsNullable;

                    field.IsForeignKey = fieldAttribute is ForeignKeyAttribute;
                    if (fieldAttribute is ForeignKeyAttribute foreignKeyAttribute)
                    {
                        field.IsExternal = typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);

                        field.AssignmentTable = foreignKeyAttribute.AssignmentTable;
                        field.RemoteColumnName = foreignKeyAttribute.RemoteColumnName;
                        field.IsManyToMany = (!string.IsNullOrWhiteSpace(field.AssignmentTable));
                    }
                }
                else
                {
                    if ((propertyInfo.GetGetMethod() == null) || (!propertyInfo.GetGetMethod().IsPublic))
                    {
                        continue;
                    }

                    field.ColumnName = propertyInfo.Name.ToUpper();
                    field.ColumnType = propertyInfo.PropertyType;
                    field.IsNullable = true;
                }

                field.Member = propertyInfo;

                fields.Add(field);
            }

            Fields = fields.ToArray();

            Internals = fields.Where(f => !f.IsExternal).ToArray();
            Externals = fields.Where(f => f.IsExternal).ToArray();
        }

        /// <summary>
        /// Member type of the Entity
        /// </summary>
        public Type Member { get; private set; }

        /// <summary>
        /// Table name of the Entity
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// All fields of the Entity
        /// </summary>
        public Field[] Fields { get; private set; }

        /// <summary>
        /// All external fields of the Entity
        /// </summary>
        public Field[] Externals { get; private set; }

        /// <summary>
        /// All internal fields of the Entity
        /// </summary>
        public Field[] Internals { get; private set; }

        /// <summary>
        /// Getter for the primary key (composite key not possible)
        /// </summary>
        public Field PrimaryKey { get; private set; }

        /// <summary>
        /// Generates SQL Select statements for this entity
        /// </summary>
        /// <param name="prefix">Prefix which is added to before each column name</param>
        /// <returns>SQL select statement (string) of this entity</returns>
        public string GetSql(string prefix = "")
        {
            string sqlQuery = "SELECT ";
            prefix = prefix.Trim();

            for (int i = 0; i < Internals.Length; i++)
            {
                if (i > 0)
                {
                    sqlQuery += ", ";
                }

                sqlQuery += prefix + Internals[i].ColumnName;
            }

            sqlQuery += $" FROM {TableName}";

            return sqlQuery;
        }

        /// <summary>
        /// Gets the field for a given column name
        /// </summary>
        /// <param name="columnName">Column name of the field</param>
        /// <returns>Field with the matching column name</returns>
        public Field GetFieldForColumn(string columnName)
        {
            columnName = columnName.ToUpper();
            foreach (Field field in Internals)
            {
                if (field.ColumnName.ToUpper() == columnName)
                {
                    return field;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the field by its name
        /// </summary>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>Field with matching name</returns>
        public Field GetFieldByName(string fieldName)
        {
            foreach (Field field in Fields)
            {
                if (field.Member.Name == fieldName)
                {
                    return field;
                }
            }

            return null;
        }
    }
}