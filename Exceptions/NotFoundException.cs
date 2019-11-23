using System;

namespace UndoneAspNetCoreApi.Exceptions
{
    public class NotFoundException : Exception
    {
        private string exceptionMessage = "Player not found";
        public string ExceptionMessage { get { return exceptionMessage; } set { exceptionMessage = value; } }
        public NotFoundException() : base() { }

        public NotFoundException(string exceptionMessage) : base(exceptionMessage)
        {
            this.exceptionMessage = exceptionMessage;
        }

        public NotFoundException(string exceptionMessage, string message) : base(message)
        {
            this.exceptionMessage = exceptionMessage;
        }

        public NotFoundException(string exceptionMessage, string message, Exception innerException) : base(message, innerException)
        {
            this.exceptionMessage = exceptionMessage;
        }
    }
}