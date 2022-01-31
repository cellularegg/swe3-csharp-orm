using System;

namespace if19b135.OrmFramework.Exceptions
{
    /// <summary>
    /// Exception is thrown if a locked object (from another user) is being locked.
    /// </summary>
    public class ObjectAlreadyLockedException : Exception
    {
        public ObjectAlreadyLockedException() : base("Object is already locked by another session.")
        {
            
        }

        public ObjectAlreadyLockedException(string message) : base(message)
        {
            
        }

        public ObjectAlreadyLockedException(string message, Exception innerEx) : base(message, innerEx)
        {
            
        }
        
    }
}