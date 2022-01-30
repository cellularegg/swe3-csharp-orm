using System;

namespace if19b135.OrmFramework.Interfaces
{
    /// <summary>
    /// Interface for a generic Cache
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Retrieves an object by Type and primary key of the cache
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="pk">Primary key of the object</param>
        /// <returns>Object or null if not found in cache</returns>
        object Get(Type t, object pk);
        
        /// <summary>
        /// Adds an object to cache
        /// </summary>
        /// <param name="obj">Object to add</param>
        void Put(object obj);
        
        /// <summary>
        /// Removes an Object from the cache
        /// </summary>
        /// <param name="obj">Object to remove</param>
        void Remove(object obj);
        
        /// <summary>
        /// Checks whether the Cache contains an object by its Type and primary key
        /// </summary>
        /// <param name="t">Type of the object</param>
        /// <param name="pk">primary key of the object</param>
        /// <returns>true or false depending on if the object is in the cache</returns>
        bool Contains(Type t, object pk);
        
        /// <summary>
        /// Checks whether the cache contains an objet
        /// </summary>
        /// <param name="obj">object to look for</param>
        /// <returns>true or false depending on if the object is in the cache</returns>
        bool Contains(object obj);
        
        /// <summary>
        /// Checks whether the Object has changed (or might has changed)
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>true or false depending if the object has or might has changed</returns>
        bool HasChanged(object obj);
    }
}