using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using if19b135.OrmFramework.Interfaces;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.Caches
{
    public class TrackingCache : DefaultCache, ICache
    {
        protected Dictionary<Type, Dictionary<object, string>> _Hashes = new();

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

        public override void Remove(object obj)
        {
            base.Remove(obj);
            _GetHash(obj.GetType()).Remove(obj.GetEntity().PrimaryKey.GetValue(obj));
        }

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