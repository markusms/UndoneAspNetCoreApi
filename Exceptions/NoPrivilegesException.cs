using System;

namespace UndoneAspNetCoreApi.Exceptions
{
    public class NoPrivilegesException : Exception
    {
        private string exceptionMessage = "No privileges to do this action!";
        public string ExceptionMessage { get { return exceptionMessage; } set { exceptionMessage = value; } }
        public NoPrivilegesException() : base() { }

        public NoPrivilegesException(string exceptionMessage) : base(exceptionMessage)
        {
            this.exceptionMessage = exceptionMessage;
        }

        public NoPrivilegesException(string exceptionMessage, string message) : base(message)
        {
            this.exceptionMessage = exceptionMessage;
        }

        public NoPrivilegesException(string exceptionMessage, string message, Exception innerException) : base(message, innerException)
        {
            this.exceptionMessage = exceptionMessage;
        }
    }
}
