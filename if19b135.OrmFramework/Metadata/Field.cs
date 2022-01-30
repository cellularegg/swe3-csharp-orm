using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using if19b135.OrmFramework.Interfaces;

namespace if19b135.OrmFramework.Metadata
{
    /// <summary>
    /// Represents a property (field) or a table column
    /// </summary>
    internal class Field
    {
        /// <summary>
        /// Constructor creates a new instance of Field
        /// </summary>
        /// <param name="entity">Corresponding Entity for this field</param>
        public Field(Entity entity)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Corresponding Entity for this field
        /// </summary>
        public Entity Entity { get; internal set; }

        /// <summary>
        /// Field's Member information
        /// </summary>
        public MemberInfo Member { get; internal set; }

        /// <summary>
        /// Gets the type of the field
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the Member is not a PropertyInfo</exception>
        public Type Type
        {
            get
            {
                if (Member is PropertyInfo propertyInfo)
                {
                    return propertyInfo.PropertyType;
                }

                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Column name of the Field
        /// </summary>
        public string ColumnName { get; internal set; }

        /// <summary>
        /// Column Type of the Field
        /// </summary>
        public Type ColumnType { get; internal set; }

        /// <summary>
        /// Flag if this Field is a primary key
        /// </summary>
        public bool IsPrimaryKey { get; internal set; }
        
        /// <summary>
        /// Flag if this field is a foreign key
        /// </summary>
        public bool IsForeignKey { get; internal set; }
        
        /// <summary>
        /// Flag if this field has NOT NULL constraints in the DB
        /// </summary>
        public bool IsNullable { get; internal set; } = false;

        /// <summary>
        /// Flag if this field is external
        /// </summary>
        public bool IsExternal { get; internal set; } = false;

        /// <summary>
        /// The Assignment table for m:n relations
        /// </summary>
        public string AssignmentTable { get; internal set; }
        
        /// <summary>
        /// Specifies the other foreign key column name in the assignment table (only used for m:n relations)
        /// </summary>
        public string RemoteColumnName { get; internal set; }
        
        /// <summary>
        /// Flag if this field is a m:n relation
        /// </summary>
        public bool IsManyToMany { get; internal set; } = false;

        /// <summary>
        /// Gets part of a SQL statement for foreign keys
        /// </summary>
        internal string _FkSql
        {
            get
            {
                if (IsManyToMany)
                {
                    return $"{Type.GenericTypeArguments[0].GetEntity().GetSql()} WHERE ID IN (SELECT " +
                           $"{RemoteColumnName} FROM {AssignmentTable} WHERE {ColumnName} = :fk)";
                }
                else
                {
                    return $"{Type.GenericTypeArguments[0].GetEntity().GetSql()}  WHERE {ColumnName} = :fk";
                }
            }
        }

        /// <summary>
        /// Gets the value of a Field from a given object
        /// </summary>
        /// <param name="obj">Object to retrieve a value from</param>
        /// <returns>The field's value of the given object</returns>
        /// <exception cref="NotSupportedException">Thrown if Member is not a PropertyInfo</exception>
        public object GetValue(object obj)
        {
            if (Member is PropertyInfo propertyInfo)
            {
                object rval = propertyInfo.GetValue(obj);

                if (rval is ILazy)
                {
                    if (!(rval is IEnumerable))
                    {
                        return rval.GetType().GetProperty("Value").GetValue(rval);
                    }
                }

                return rval;
            }

            throw new NotSupportedException("Member type not supported.");
        }

        /// <summary>
        /// Sets the value of a Field for a given object
        /// </summary>
        /// <param name="obj">Object to set the value in</param>
        /// <param name="value">The field's value to set</param>
        /// <exception cref="NotSupportedException">Thrown if Member is not a PropertyInfo</exception>
        public void SetValue(object obj, object value)
        {
            if (Member is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(obj, value);
            }
            else
            {
                throw new NotSupportedException("Member type is not supported");
            }
        }

        /// <summary>
        /// Converts a value to the column data type
        /// </summary>
        /// <param name="value">Object with value to convert</param>
        /// <returns>Converted value</returns>
        public object ToColumnType(object value)
        {
            if (IsForeignKey)
            {
                if (value == null)
                {
                    return null;
                }

                Type t = typeof(ILazy).IsAssignableFrom(Type) ? Type.GenericTypeArguments[0] : Type;

                return t.GetEntity().PrimaryKey.ToColumnType(t.GetEntity().PrimaryKey.GetValue(value));
            }

            if (Type == ColumnType)
            {
                return value;
            }

            if (value is bool b)
            {
                if (ColumnType == typeof(int))
                {
                    return b ? 1 : 0;
                }

                if (ColumnType == typeof(short))
                {
                    return b ? 1 : 0;
                }

                if (ColumnType == typeof(long))
                {
                    return b ? 1 : 0;
                }
            }

            return value;
        }

        /// <summary>
        /// Converts a value to the field data type
        /// </summary>
        /// <param name="value">Object with value to convert</param>
        /// <param name="localCache">Local cache to stop stack overflow for circle dependencies</param>
        /// <returns>Converted value</returns>
        public object ToFieldType(object value, ICollection<object> localCache)
        {
            if (IsForeignKey)
            {
                if (typeof(ILazy).IsAssignableFrom(Type))
                {
                    return Activator.CreateInstance(Type, value);
                }

                return Orm._CreateObject(Type, value, localCache);
            }

            if (Type == typeof(bool))
            {
                if (value is int i)
                {
                    return i != 0;
                }

                if (value is short s)
                {
                    return s != 0;
                }

                if (value is long l)
                {
                    return l != 0;
                }
            }

            if (Type == typeof(int))
            {
                return Convert.ToInt32(value);
            }

            if (Type == typeof(short))
            {
                return Convert.ToInt32(value);
            }

            if (Type == typeof(long))
            {
                return Convert.ToInt32(value);
            }

            if (Type.IsEnum)
            {
                return Enum.ToObject(Type, value);
            }

            return value;
        }

        /// <summary>
        /// Updates the foreign key references
        /// </summary>
        /// <param name="obj">Object which references should be updated</param>
        public void UpdateReferences(object obj)
        {
            if (!IsExternal) return;

            Type innerType = Type.GetGenericArguments()[0];
            Entity innerEntity = innerType.GetEntity();
            object pk = Entity.PrimaryKey.ToColumnType(Entity.PrimaryKey.GetValue(obj));

            if (IsManyToMany)
            {
                IDbCommand cmd = Orm.Connection.CreateCommand();
                cmd.CommandText = $"DELETE FROM {AssignmentTable} WHERE {ColumnName} = :pk";
                IDataParameter p = cmd.CreateParameter();
                p.ParameterName = ":pk";
                p.Value = pk;
                cmd.Parameters.Add(p);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                if (GetValue(obj) != null)
                {
                    foreach (object i in (IEnumerable)GetValue(obj))
                    {
                        cmd = Orm.Connection.CreateCommand();
                        cmd.CommandText =
                            $"INSERT INTO {AssignmentTable} ({ColumnName}, {RemoteColumnName}) VALUES (:pk, :fk)";
                        p = cmd.CreateParameter();
                        p.ParameterName = ":pk";
                        p.Value = pk;
                        cmd.Parameters.Add(p);

                        p = cmd.CreateParameter();
                        p.ParameterName = ":fk";
                        p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));
                        cmd.Parameters.Add(p);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
            }
            else
            {
                Field remoteField = innerEntity.GetFieldForColumn(ColumnName);
                if (remoteField.IsNullable)
                {
                    IDbCommand cmd = Orm.Connection.CreateCommand();
                    cmd.CommandText =
                        $"UPDATE {innerEntity.TableName} SET {ColumnName} = NULL WHERE {ColumnName} = :fk";

                    IDataParameter p = cmd.CreateParameter();
                    p.ParameterName = ":fk";
                    p.Value = pk;
                    cmd.Parameters.Add(p);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                if (GetValue(obj) != null)
                {
                    foreach (object i in (IEnumerable)GetValue(obj))
                    {
                        remoteField.SetValue(i, obj);

                        IDbCommand cmd = Orm.Connection.CreateCommand();
                        cmd.CommandText =
                            $"UPDATE {innerEntity.TableName} SET {ColumnName} = :fk " +
                            $"WHERE {innerEntity.PrimaryKey.ColumnName} = :pk";
                        IDataParameter p = cmd.CreateParameter();
                        p.ParameterName = ":fk";
                        p.Value = pk;
                        cmd.Parameters.Add(p);

                        p = cmd.CreateParameter();
                        p.ParameterName = ":pk";
                        p.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));
                        cmd.Parameters.Add(p);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Fills a list for a foreign key
        /// </summary>
        /// <param name="list">List to fill</param>
        /// <param name="obj">Object that contains the foreign key</param>
        /// <param name="localCache">Local cache to stop stack overflow for circle dependencies</param>
        /// <returns>List with foreign key objects</returns>
        public object Fill(object list, object obj, ICollection<object> localCache)
        {
            // IDbCommand cmd = Orm.Connection.CreateCommand();
            //
            // if (IsManyToMany)
            // {
            //     cmd.CommandText = $"{Type.GenericTypeArguments[0].GetEntity().GetSql()} WHERE ID IN (SELECT " +
            //                       $"{RemoteColumnName} FROM {AssignmentTable} WHERE {ColumnName} = :fk)";
            // }
            // else
            // {
            //     cmd.CommandText = $"{Type.GenericTypeArguments[0].GetEntity().GetSql()}  WHERE {ColumnName} = :fk";
            // }
            //
            // IDataParameter p = cmd.CreateParameter();
            // p.ParameterName = ":fk";
            // // Optional to field type
            // p.Value = Entity.PrimaryKey.GetValue(obj);
            // cmd.Parameters.Add(p);
            // IDataReader reader = cmd.ExecuteReader();
            // while (reader.Read())
            // {
            //     list.GetType().GetMethod("Add")
            //         .Invoke(list, new object[] { Orm._CreateObject(Type.GenericTypeArguments[0], reader, localCache) });
            // }
            //
            // reader.Close();
            // reader.Dispose();
            // cmd.Dispose();
            Orm._FillList(Type.GenericTypeArguments[0], list, _FkSql,
                new Tuple<string, object>[] { new(":fk", Entity.PrimaryKey.GetValue(obj)) },
                localCache);

            return list;
        }
    }
}