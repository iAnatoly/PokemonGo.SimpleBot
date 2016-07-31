using System;
using System.Runtime.Serialization;

namespace PokemonGo.SimpleBot.Exceptions
{
    [Serializable]
    internal class RepeatedInvalidResponseException : Exception
    {
        public RepeatedInvalidResponseException()
        {
        }

        public RepeatedInvalidResponseException(string message) : base(message)
        {
        }

        public RepeatedInvalidResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RepeatedInvalidResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}