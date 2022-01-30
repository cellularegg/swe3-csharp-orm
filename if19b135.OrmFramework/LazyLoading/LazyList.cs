using System;
using System.Collections;
using System.Collections.Generic;
using if19b135.OrmFramework.Interfaces;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.LazyLoading
{
    /// <summary>
    /// Lazily loaded List of T
    /// </summary>
    /// <typeparam name="T">Lazy Lists' type</typeparam>
    public class LazyList<T> : IList<T>, ILazy
    {
        /// <summary>
        /// List Values which are lazily loaded
        /// </summary>
        protected List<T> _InternalItems = null;

        /// <summary>
        /// SQL statement to load list
        /// </summary>
        protected string _Sql;


        /// <summary>
        /// Parameters for SQL statement
        /// </summary>
        protected ICollection<Tuple<string, object>> _Params;

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="sql">SQL statement to load list</param>
        /// <param name="parameters">Parameters for SQL Statement</param>
        internal protected LazyList(string sql, ICollection<Tuple<string, object>> parameters)
        {
            _Sql = sql;
            _Params = parameters;
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="obj">Object to which contains the field to be loaded</param>
        /// <param name="fieldName">Name of the field to load</param>
        public LazyList(object obj, string fieldName)
        {
            Field f = obj.GetEntity().GetFieldByName(fieldName);
            _Sql = f._FkSql;
            _Params = new Tuple<string, object>[]
                { new Tuple<string, object>(":fk", f.Entity.PrimaryKey.GetValue(obj)) };
        }

        /// <summary>
        /// Getter for the Items of the list (lazily loaded)
        /// </summary>
        protected List<T> _Items
        {
            get
            {
                if (_InternalItems == null)
                {
                    _InternalItems = new List<T>();
                    Orm._FillList(typeof(T), _InternalItems, _Sql, _Params);
                }

                return _InternalItems;
            }
        }


        // Implementation of IList<T>

        /// <summary>
        /// Gets or sets an item by its index
        /// </summary>
        /// <param name="index">Index of the item</param>
        /// <returns>Item at given index</returns>
        public T this[int index]
        {
            get { return _Items[index]; }
            set { _Items[index] = value; }
        }

        /// <summary>
        /// Gets the number of items in the list
        /// </summary>
        public int Count
        {
            get { return _Items.Count; }
        }

        /// <summary>
        /// Gets if the list is read only
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList<T>)_Items).IsReadOnly; }
        }

        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(T item)
        {
            _Items.Add(item);
        }

        /// <summary>
        /// Clears the list / Deletes everything in the list.
        /// </summary>
        public void Clear()
        {
            _Items.Clear();
        }

        /// <summary>
        /// Checks whether an item is in the list
        /// </summary>
        /// <param name="item">Item to look for</param>
        /// <returns>true or false depending on if the item is in the list</returns>
        public bool Contains(T item)
        {
            return _Items.Contains(item);
        }


        /// <summary>
        /// Copies the List to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Index to start copying</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the enumerator for of the list
        /// </summary>
        /// <returns>Enumerator of the list</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of an item
        /// </summary>
        /// <param name="item">Item to look for</param>
        /// <returns>Index of the item</returns>
        public int IndexOf(T item)
        {
            return _Items.IndexOf(item);
        }

        /// <summary>
        /// Insert an item at a given index
        /// </summary>
        /// <param name="index">Index to insert at</param>
        /// <param name="item">Item to insert</param>
        public void Insert(int index, T item)
        {
            _Items.Insert(index, item);
        }

        /// <summary>
        /// Removes an item from the list
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>true or false depending if the deletion succeeded</returns>
        public bool Remove(T item)
        {
            return _Items.Remove(item);
        }

        /// <summary>
        /// Removes an item at a given index
        /// </summary>
        /// <param name="index">Index to remove</param>
        public void RemoveAt(int index)
        {
            _Items.RemoveAt(index);
        }

        /// <summary>
        /// Gets the enumerator for of the list
        /// </summary>
        /// <returns>Enumerator of the list</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
    }
}