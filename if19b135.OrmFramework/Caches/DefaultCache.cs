using System;
using System.Collections.Generic;
using if19b135.OrmFramework.Interfaces;

namespace if19b135.OrmFramework.Caches
{
    /// <summary>
    /// Simple Cache implementation with no implementation to track changes
    /// </summary>
    public class DefaultCache : ICache
    {
        /// <summary>
        /// Dictionary of Caches per Type
        /// </summary>
        protected Dictionary<Type, Dictionary<object, object>> _Caches = new();

        /// <summary>
        /// Getter for the cache for a given Type. Creates new cache if type has no cache yet.
        /// </summary>
        /// <param name="t">Type to retrieve/create a cache for</param>
        /// <returns>Cache for type t</returns>
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
        
        // Implementation if ICache
        
        /// <summary>
        /// Retrieves an object by Type and primary key of the cache
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="pk">Primary key of the object</param>
        /// <returns>Object or null if not found in cache</returns>
        public virtual object Get(Type t, object pk)
        {
            Dictionary<object, object> cache = _GetCache(t);
            if (cache.ContainsKey(pk))
            {
                return cache[pk];
            }

            return null;
        }

        /// <summary>
        /// Adds an object to cache
        /// </summary>
        /// <param name="obj">Object to add</param>
        public virtual void Put(object obj)
        {
            if (obj != null)
            {
                _GetCache(obj.GetType())[obj.GetEntity().PrimaryKey.GetValue(obj)] = obj;
            }
        }

        /// <summary>
        /// Removes an Object from the cache
        /// </summary>
        /// <param name="obj">Object to remove</param>
        public virtual void Remove(object obj)
        {
            _GetCache(obj.GetType()).Remove(obj.GetEntity().PrimaryKey.GetValue(obj));
        }
        
        /// <summary>
        /// Checks whether the Cache contains an object by its Type and primary key
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="pk">primary key of the object</param>
        /// <returns>true or false depending on if the object is in the cache</returns>
        public virtual bool Contains(Type t, object pk)
        {
            return _GetCache(t).ContainsKey(pk);
        }

        /// <summary>
        /// Checks whether the cache contains an objet
        /// </summary>
        /// <param name="obj">object to look for</param>
        /// <returns>true or false depending on if the object is in the cache</returns>
        public virtual bool Contains(object obj)
        {
            return Contains(obj.GetType(), obj.GetEntity().PrimaryKey.GetValue(obj));
        }

        /// <summary>
        /// Checks whether the Object has changed (or might has changed)
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>true, since cache has no way of checking if an object has changed</returns>
        public virtual bool HasChanged(object obj)
        {
            return true;
        }
    }
}