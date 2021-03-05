using System;

namespace MongoDbMonitor.Commands.Exceptions
{
    internal class SendNotificationFailedException : Exception
    {
        public SendNotificationFailedException(Exception innerException) :
            base("Sending notification failed.", innerException)
        {
        }
    }
}
