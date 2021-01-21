using System;
using System.Runtime.Serialization;

namespace Core.StateHandler
{
    [Serializable]
    internal class IllegalStateException : Exception
    {
        private State state;

        public IllegalStateException()
        {
        }

        public IllegalStateException(State state) : base(message: $"Illegal state encountered: {state}")
        {
        }

        public IllegalStateException(string message) : base($"Illegal state encountered: {message}")
        {
        }

        public IllegalStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IllegalStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}