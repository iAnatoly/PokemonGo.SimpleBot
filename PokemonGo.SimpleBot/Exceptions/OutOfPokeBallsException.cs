using System;
using System.Runtime.Serialization;

namespace PokemonGo.SimpleBot.Exceptions
{
    [Serializable]
    internal class OutOfPokeBallsException : Exception
    {
        public OutOfPokeBallsException()
        {
        }

        public OutOfPokeBallsException(string message) : base(message)
        {
        }

        public OutOfPokeBallsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OutOfPokeBallsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}