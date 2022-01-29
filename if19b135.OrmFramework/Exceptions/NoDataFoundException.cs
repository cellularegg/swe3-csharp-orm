using System;

namespace if19b135.OrmFramework.Exceptions
{
    public class NoDataFoundException : Exception
    {
        public NoDataFoundException()
        {
        }

        public NoDataFoundException(string message) : base(message)
        {
        }

        public NoDataFoundException(string message, Exception innerEx) : base(message, innerEx)
        {
        }
    }
}