using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using if19b135.OrmFramework.Interfaces;

namespace if19b135.OrmFramework.Metadata
{
    // represents a property / table column
    internal class Field
    {
        public Field(Entity entity)
        {
            this.Entity = entity;
        }

        public Entity Entity { get; internal set; }

        public MemberInfo Member { get; internal set; }

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

        public string ColumnName { get; internal set; }

        public Type ColumnType { get; internal set; }

        public bool IsPrimaryKey { get; internal set; }
        public bool IsForeignKey { get; internal set; }
        public bool IsNullable { get; internal set; } = false;

        public bool IsExternal { get; internal set; } = false;

        public string AssignmentTable { get; internal set; }
        public string RemoteColumnName { get; internal set; }
        public bool IsManyToMany { get; internal set; } = false;

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

        public object GetValue(object obj)
        {
                        if(Member is PropertyInfo) 
            {
                object rval = ((PropertyInfo) Member).GetValue(obj);

                if(rval is ILazy)
                {
                    if(!(rval is IEnumerable)) { return rval.GetType().GetProperty("Value").GetValue(rval); }
                }

                return rval;
            }

            throw new NotSupportedException("Member type not supported.");
        }

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