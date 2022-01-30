using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using if19b135.OrmFramework.Exceptions;
using if19b135.OrmFramework.Interfaces;
using if19b135.OrmFramework.Metadata;
using if19b135.OrmFramework.Query;

namespace if19b135.OrmFramework
{
    /// <summary>
    /// Orm Class - Interface for the user to access the Orm features thorugh
    /// </summary>
    public static class Orm
    {
        /// <summary>
        /// Entities of the Orm
        /// </summary>
        private static Dictionary<Type, Entity> _Entities = new Dictionary<Type, Entity>();

        /// <summary>
        /// Connection to the Database
        /// </summary>
        public static IDbConnection Connection { get; set; }

        /// <summary>
        /// Cache
        /// </summary>
        public static ICache Cache { get; set; }

        /// <summary>
        /// Returns an Entity for a specific object
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>Entity</returns>
        internal static Entity GetEntity(this object o)
        {
            Type t = o is Type type ? type : o.GetType();

            if (!_Entities.ContainsKey(t))
            {
                _Entities.Add(t, new Entity(t));
            }

            return _Entities[t];
        }

        /// <summary>
        /// Gets Child Classes of a given parent
        /// </summary>
        /// <param name="t">Parent type</param>
        /// <returns>Array of non abstract child Types</returns>
        internal static Type[] GetChildTypes(this Type t)
        {
            List<Type> childTypes = new List<Type>();
            foreach (Type entityKey in _Entities.Keys)
            {
                if (t.IsAssignableFrom(entityKey) && !entityKey.IsAbstract)
                {
                    childTypes.Add(entityKey);
                }
            }

            return childTypes.ToArray();
        }

        /// <summary>
        /// Retrieves an object from the Database
        /// </summary>
        /// <param name="pk">Primary key of the object</param>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <returns>Object with given primary key</returns>
        public static T Get<T>(object pk)
        {
            return (T)_CreateObject(typeof(T), pk, null);
        }

        public static Query<T> From<T>()
        {
            return new Query<T>(null);
        }

        /// <summary>
        /// Saves an object to the database
        /// </summary>
        /// <param name="obj">Object to save</param>
        public static void Save(object obj)
        {
            if (Cache != null && !Cache.HasChanged(obj))
            {
                return;
            }

            Entity ent = obj.GetEntity();
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ($"INSERT INTO {ent.TableName} (");

            string update = $"ON CONFLICT ({ent.PrimaryKey.ColumnName}) DO UPDATE SET ";
            string inserts = "";

            IDataParameter p;
            bool first = true;
            for (int i = 0; i < ent.Internals.Length; i++)
            {
                if (i > 0)
                {
                    cmd.CommandText += ", ";
                    inserts += ", ";
                }

                cmd.CommandText += ent.Internals[i].ColumnName;
                inserts += $":v{i}";
                p = cmd.CreateParameter();
                p.ParameterName = $":v{i}";
                p.Value = ent.Internals[i].ToColumnType(ent.Internals[i].GetValue(obj));
                cmd.Parameters.Add(p);
                if (!ent.Internals[i].IsPrimaryKey)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        update += ", ";
                    }

                    update += $"{ent.Internals[i].ColumnName} = :w{i}";

                    p = cmd.CreateParameter();
                    p.ParameterName = $":w{i}";
                    p.Value = ent.Internals[i].ToColumnType(ent.Internals[i].GetValue(obj));
                    cmd.Parameters.Add(p);
                }
            }

            cmd.CommandText += $") VALUES ({inserts}) {update}";
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            foreach (Field extField in ent.Externals)
            {
                extField.UpdateReferences(obj);
            }

            if (Cache != null)
            {
                Cache.Put(obj);
            }
        }


        /// <summary>
        /// Deletes an object from the database
        /// </summary>
        /// <param name="obj">Object to delete</param>
        public static void Delete(object obj)
        {
            Entity ent = obj.GetEntity();

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {ent.TableName} WHERE {ent.PrimaryKey.ColumnName} = :pk";
            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":pk";
            p.Value = ent.PrimaryKey.GetValue(obj);
            cmd.Parameters.Add(p);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (Cache != null)
            {
                Cache.Remove(obj);
            }
        }

        /// <summary>
        /// Creates an object of Type t for a given data reader
        /// </summary>
        /// <param name="t">Object's Type</param>
        /// <param name="reader">Data reader to read from</param>
        /// <returns>Object from database</returns>
        internal static object _CreateObject(Type t, IDataReader reader, ICollection<object> localCache)
        {
            Entity e = t.GetEntity();

            object obj = _SearchCache(t,
                e.PrimaryKey.ToFieldType(reader.GetValue(reader.GetOrdinal(e.PrimaryKey.ColumnName)), localCache),
                localCache);

            if (obj == null)
            {
                if (localCache == null)
                {
                    localCache = new List<object>();
                }

                // Type t requires parameterless constructor
                obj = Activator.CreateInstance(t);
                localCache.Add(obj);
            }

            foreach (Field internalField in e.Internals)
            {
                internalField.SetValue(obj,
                    internalField.ToFieldType(reader.GetValue(reader.GetOrdinal(internalField.ColumnName)),
                        localCache));
            }

            foreach (Field externalField in e.Externals)
            {
                if (typeof(ILazy).IsAssignableFrom(externalField.Type))
                {
                    externalField.SetValue(obj,
                        Activator.CreateInstance(externalField.Type, obj, externalField.Member.Name));
                }
                else
                {
                    externalField.SetValue(obj,
                        externalField.Fill(Activator.CreateInstance(externalField.Type), obj, localCache));
                }
            }


            return obj;
        }

        /// <summary>
        /// Creates an object of Type t for a given primary key
        /// </summary>
        /// <param name="t">Object's Type</param>
        /// <param name="pk">Object's primary key</param>
        /// <returns>Object from database</returns>
        /// <exception cref="Exception">Exception if no data is found</exception>
        internal static object _CreateObject(Type t, object pk, ICollection<object> localCache)
        {
            object obj = _SearchCache(t, pk, localCache);

            IDbCommand cmd = Connection.CreateCommand();

            cmd.CommandText = $"{t.GetEntity().GetSql()} WHERE {t.GetEntity().PrimaryKey.ColumnName} = :pk";

            IDataParameter p = cmd.CreateParameter();
            p.ParameterName = ":pk";
            // Optional to column type, however not required since PK ist mostly just string or a number
            p.Value = pk;
            cmd.Parameters.Add(p);

            IDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                obj = _CreateObject(t, reader, localCache);
            }

            reader.Close();
            reader.Dispose();
            cmd.Dispose();

            if (Cache != null)
            {
                Cache.Put(obj);
            }


            return obj;
        }

        /// <summary>
        /// Searches Cache of Type by Primary key
        /// </summary>
        /// <param name="t">Type of object to retrieve</param>
        /// <param name="pk">Primary key of the object</param>
        /// <param name="localCache">Local cache</param>
        /// <returns>Cached Object or null if not in cache</returns>
        internal static object _SearchCache(Type t, object pk, ICollection<object> localCache)
        {
            if (Cache != null && Cache.Contains(t, pk))
            {
                return Cache.Get(t, pk);
            }

            if (localCache != null)
            {
                foreach (object obj in localCache)
                {
                    if (obj.GetType() != t)
                    {
                        continue;
                    }

                    if (t.GetEntity().PrimaryKey.GetValue(obj).Equals(pk))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        internal static void _FillList(Type t, object list, IDataReader reader, ICollection<object> localCache = null)
        {
            while (reader.Read())
            {
                list.GetType().GetMethod("Add").Invoke(list, new object[] { _CreateObject(t, reader, localCache) });
            }
        }

        internal static void _FillList(Type t, object list, string sql, IEnumerable<Tuple<string, object>> parameters,
            ICollection<object> localCache = null)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            IDataParameter p;
            foreach (Tuple<string, object> parameter in parameters)
            {
                p = cmd.CreateParameter();
                p.ParameterName = parameter.Item1;
                p.Value = parameter.Item2;
                cmd.Parameters.Add(p);
            }

            IDataReader reader = cmd.ExecuteReader();
            _FillList(t, list, reader, localCache);
            reader.Close();
            reader.Dispose();
            cmd.Dispose();
        }
    }
}