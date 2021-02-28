using MediatR;
using MongoDB.Bson;
using MongoDbMonitor.Commands.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbMonitor.Commands.SendNotification
{
    internal class SendNotificationRequest : IRequest, IOnRequestProcessingError
    {
        public string CollectionName { get; set; }

        public ObjectId Id { get; set; }

        public void OnError([NotNull] Exception ex)
        {
        }
    }
}