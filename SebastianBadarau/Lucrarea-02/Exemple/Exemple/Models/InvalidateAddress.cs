using System;
using System.Runtime.Serialization;

namespace Exemple.Domain.Models
{
    [Serializable]
    internal class InvalidateAddressException : Exception
    {
        public InvalidateAddressException()
        {
        }

        public InvalidateAddressException(string? message) : base(message)
        {
        }

        public InvalidateAddressException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidateAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}