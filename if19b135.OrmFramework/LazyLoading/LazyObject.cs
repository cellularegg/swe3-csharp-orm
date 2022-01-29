using if19b135.OrmFramework.Interfaces;

namespace if19b135.OrmFramework.LazyLoading
{
    public class LazyObject<T> : ILazy
    {
        protected object _Pk;

        protected T _Value;

        protected bool _Initialized = false;
        
        public LazyObject(object pk = null)
        {
            _Pk = pk;
        }

        public T Value
        {
            get
            {
                if (!_Initialized)
                {
                    _Value = Orm.Get<T>(_Pk);
                    _Initialized = true;
                }

                return _Value;
            }
            set
            {
                _Value = value;
                _Initialized = true;
            }
        }

        public static implicit operator T(LazyObject<T> lazy)
        {
            return lazy._Value;
        }

        public static implicit operator LazyObject<T>(T obj)
        {
            LazyObject<T> lazy = new LazyObject<T>();
            lazy.Value = obj;
            return lazy;
        }

    }
}