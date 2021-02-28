using System;

namespace MongoDbMonitor.Commands.Exceptions
{
    internal class InvalidRequestTypeException : Exception
    {
        public InvalidRequestTypeException(string assemblyName, string fullQualifiedName)
            :base ($"Assembly: {assemblyName}, Full name: {fullQualifiedName} can't be resolved.")
        {
        }
    }
}
