using MediatR;
using MongoDB.Bson;

namespace MongoDbMonitor.Commands.SendNotification
{
    internal class SendNotificationRequest : IRequest
    {
        public string CollectionName { get; set; }

        public ObjectId Id { get; set; }
    }
}