using System;

namespace if19b135.OrmFramework.Interfaces
{
    public interface ICache
    {
        object Get(Type t, object pk);
        void Put(object obj);
        void Remove(object obj);
        bool Contains(Type t, object pk);
        bool Contains(object obj);
        bool HasChanged(object obj);
    }
}