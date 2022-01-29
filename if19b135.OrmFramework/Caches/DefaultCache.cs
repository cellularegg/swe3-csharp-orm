using System;
using System.Collections.Generic;
using if19b135.OrmFramework.Interfaces;

namespace if19b135.OrmFramework.Caches
{
    public class DefaultCache : ICache
    {
        protected Dictionary<Type, Dictionary<object, object>> _Caches = new();

        protected Dictionary<object, object> _GetCache(Type t)
        {
            if (_Caches.ContainsKey(t))
            {
                return _Caches[t];
            }

            Dictionary<object, object> cache = new Dictionary<object, object>();
            _Caches.Add(t, cache);
            return cache;
        }

        public virtual object Get(Type t, object pk)
        {
            Dictionary<object, object> cache = _GetCache(t);
            if (cache.ContainsKey(pk))
            {
                return cache[pk];
            }

            return null;
        }

        public virtual void Put(object obj)
        {
            if (obj != null)
            {
                _GetCache(obj.GetType())[obj.GetEntity().PrimaryKey.GetValue(obj)] = obj;
            }
        }

        public virtual void Remove(object obj)
        {
            _GetCache(obj.GetType()).Remove(obj.GetEntity().PrimaryKey.GetValue(obj));
        }

        public virtual bool Contains(Type t, object pk)
        {
            return _GetCache(t).ContainsKey(pk);
        }

        public virtual bool Contains(object obj)
        {
            return Contains(obj.GetType(), obj.GetEntity().PrimaryKey.GetValue(obj));
        }

        public virtual bool HasChanged(object obj)
        {
            return true;
        }
    }
}