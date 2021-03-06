using System;

namespace MongoDbMonitor.Commands.Exceptions
{
    internal sealed class InvalidObjectIdException : Exception
    {
        public InvalidObjectIdException(string value) : base($"Value: {value} is not a valid MongoDb identifier")
        {
        }
    }
}
