using System;
using System.Collections;
using System.Collections.Generic;
using if19b135.OrmFramework.Interfaces;
using if19b135.OrmFramework.Metadata;

namespace if19b135.OrmFramework.LazyLoading
{
    public class LazyList<T> : IList<T>, ILazy
    {
        protected List<T> _InternalItems = null;

        protected string _Sql;

        protected ICollection<Tuple<string, object>> _Params;


        internal protected LazyList(string sql, ICollection<Tuple<string, object>> parameters)
        {
            _Sql = sql;
            _Params = parameters;
        }

        public LazyList(object obj, string fieldName)
        {
            Field f = obj.GetEntity().GetFieldByName(fieldName);
            _Sql = f._FkSql;
            _Params = new Tuple<string, object>[]
                { new Tuple<string, object>(":fk", f.Entity.PrimaryKey.GetValue(obj)) };
        }


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

        public T this[int index]
        {
            get { return _Items[index]; }
            set { _Items[index] = value; }
        }


        public int Count
        {
            get { return _Items.Count; }
        }


        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList<T>)_Items).IsReadOnly; }
        }


        public void Add(T item)
        {
            _Items.Add(item);
        }


        public void Clear()
        {
            _Items.Clear();
        }


        public bool Contains(T item)
        {
            return _Items.Contains(item);
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }


        public int IndexOf(T item)
        {
            return _Items.IndexOf(item);
        }


        public void Insert(int index, T item)
        {
            _Items.Insert(index, item);
        }


        public bool Remove(T item)
        {
            return _Items.Remove(item);
        }


        public void RemoveAt(int index)
        {
            _Items.RemoveAt(index);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
    }
}