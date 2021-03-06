using System;

namespace MongoDbMonitor.Commands.Exceptions
{
    internal class MissingRequiredPropertyException : Exception
    {
        public MissingRequiredPropertyException(string type, string propertyName)
            : base($"Type: {type} is missing required property: {propertyName}")
        {

        }
    }
}
