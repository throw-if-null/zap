using System;

namespace MongoDbMonitor.Commands.Exceptions
{
    internal class SendNotificationFailedException : Exception
    {
        public SendNotificationFailedException() : base("Sending notification for failed")
        {
        }
    }
}
