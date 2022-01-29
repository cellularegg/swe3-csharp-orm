using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using if19b135.OrmFramework.Attributes;

namespace if19b135.OrmFramework.Metadata
{
    // represents a class / table
    internal class Entity
    {
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

        public Type Member { get; private set; }

        public string TableName { get; private set; }

        public Field[] Fields { get; private set; }

        public Field[] Externals { get; private set; }

        public Field[] Internals { get; private set; }

        /// <summary>
        /// Getter for the primary key (composite key not possible)
        /// </summary>
        public Field PrimaryKey { get; private set; }

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