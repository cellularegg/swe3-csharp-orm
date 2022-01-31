namespace if19b135.OrmFramework.Interfaces
{
    /// <summary>
    /// Locking interface
    /// </summary>
    public interface ILocking
    {
        /// <summary>
        /// Lock a given object
        /// </summary>
        /// <param name="obj">Object to lock</param>
        void Lock(object obj);

        /// <summary>
        /// Release / Unlock a given object
        /// </summary>
        /// <param name="obj">Object to unlock/release</param>
        void Release(object obj);
    }
}