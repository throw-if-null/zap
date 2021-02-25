using MediatR;
using MongoDB.Bson;

namespace MongoDbFunction.Commands.SendNotification
{
    public sealed class SendNotificationRequest : IRequest
    {
        public string CollectionName { get; set; }

        public ObjectId Id { get; set; }
    }
}
