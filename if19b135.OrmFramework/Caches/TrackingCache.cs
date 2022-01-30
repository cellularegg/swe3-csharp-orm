using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using if19b135.OrmFramework.Interfaces;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Caches
{
    /// <summary>
    /// Implementation of a cache that is able to track changes
    /// </summary>
    public class TrackingCache : DefaultCache, ICache
    {
        /// <summary>
        /// Dictionary hashes per Type
        /// </summary>
        protected Dictionary<Type, Dictionary<object, string>> _Hashes = new();

        /// <summary>
        /// Getter for the hash dictionary for a given Type.
        /// Creates new hash dictionary if type has no hash dictionary yet.
        /// </summary>
        /// <param name="t">Type to retrieve/create a hash dictionary for</param>
        /// <returns>Hash dictionary for type t</returns>
        protected Dictionary<object, string> _GetHash(Type t)
        {
            if (_Hashes.ContainsKey(t))
            {
                return _Hashes[t];
            }

            Dictionary<object, string> hash = new Dictionary<object, string>();
            _Hashes.Add(t, hash);
            return hash;
        }

        /// <summary>
        /// Computes the hash for a given object (SHA256 hash)
        /// </summary>
        /// <param name="obj">Object to hash</param>
        /// <returns>SHA256 hash of the object</returns>
        protected string _ComputeHash(object obj)
        {
            string hash = "";

            foreach (Field intField in obj.GetEntity().Internals)
            {
                object val = intField.GetValue(obj);
                if (val != null)
                {
                    if (!intField.IsForeignKey)
                    {
                        hash += $"{intField.ColumnName}={val};";
                    }
                    else
                    {
                        var tmpEnt = val.GetEntity();
                        hash += $"{intField.ColumnName}={val.GetEntity().PrimaryKey.GetValue(val)};";
                    }
                }
            }

            foreach (Field extField in obj.GetEntity().Externals)
            {
                IEnumerable vals = (IEnumerable)extField.GetValue(obj);
                if (vals != null)
                {
                    hash += $"{extField.ColumnName}=";
                    foreach (object val in vals)
                    {
                        hash += $"{val.GetEntity().PrimaryKey.GetValue(val)},";
                    }
                }
            }
            
            // create SHA 256 from long string
            return Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(hash)));
        }

        // Implementation of ICache
        
        /// <summary>
        /// Adds an object to cache
        /// </summary>
        /// <param name="obj">Object to add</param>
        public override void Put(object obj)
        {
            base.Put(obj);
            if (obj != null)
            {
                Type t = obj.GetType();
                var pk = obj.GetEntity().PrimaryKey.GetValue(obj);
                _GetHash(obj.GetType())[obj.GetEntity().PrimaryKey.GetValue(obj)] = _ComputeHash(obj);
            }
        }

        /// <summary>
        /// Removes an Object from the cache
        /// </summary>
        /// <param name="obj">Object to remove</param>
        public override void Remove(object obj)
        {
            base.Remove(obj);
            _GetHash(obj.GetType()).Remove(obj.GetEntity().PrimaryKey.GetValue(obj));
        }

        /// <summary>
        /// Checks whether the Object has changed (or might has changed)
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>true or false depending if the object has or might has changed</returns>
        public override bool HasChanged(object obj)
        {
            Dictionary<object, string> hashes = _GetHash(obj.GetType());
            object pk = obj.GetEntity().PrimaryKey.GetValue(obj);

            if (hashes.ContainsKey(pk))
            {
                return hashes[pk] != _ComputeHash(obj);
            }

            return true;
        }
    }
}