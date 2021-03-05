using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.Common.Responses;

namespace MongoDbMonitor.Commands.SendNotification
{
    internal class SendNotificationRequest : IRequest<ProcessingStatusResponse>
    {
        public string CollectionName { get; set; }

        public ObjectId Id { get; set; }
    }
}