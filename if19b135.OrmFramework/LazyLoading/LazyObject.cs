using if19b135.OrmFramework.Interfaces;

namespace if19b135.OrmFramework.LazyLoading
{
    /// <summary>
    /// Lazily loaded object
    /// </summary>
    /// <typeparam name="T">Type of the lazy object</typeparam>
    public class LazyObject<T> : ILazy
    {
        /// <summary>
        /// Primary key of the object
        /// </summary>
        protected object _Pk;

        /// <summary>
        /// Value of the object
        /// </summary>
        protected T _Value;

        /// <summary>
        /// Flag if the object has been loaded
        /// </summary>
        protected bool _Initialized = false;


        /// <summary>
        /// Constructor that creates an instance by a given primary key
        /// </summary>
        /// <param name="pk">Primary key of the object</param>
        public LazyObject(object pk = null)
        {
            _Pk = pk;
        }

        /// <summary>
        /// Getter and setter for the objects Value (lazily loaded)
        /// </summary>
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

        /// <summary>
        /// Implicit operator to enable the interchangeable use of LazyObject<T> and T
        /// </summary>
        /// <param name="lazy">Lazy Object</param>
        /// <returns>Value of Lazy Object</returns>
        public static implicit operator T(LazyObject<T> lazy)
        {
            return lazy._Value;
        }

        /// <summary>
        /// Implicit operator to enable the interchangeable use of LazyObject<T> and T
        /// </summary>
        /// <param name="obj">Regular Object</param>
        /// <returns>Lazy Object</returns>
        public static implicit operator LazyObject<T>(T obj)
        {
            LazyObject<T> lazy = new LazyObject<T>();
            lazy.Value = obj;
            return lazy;
        }
    }
}