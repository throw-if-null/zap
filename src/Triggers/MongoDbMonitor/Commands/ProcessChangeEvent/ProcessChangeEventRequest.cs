using MediatR;
using MongoDB.Driver;

namespace MongoDbMonitor.Commands.ProcessChangeEvent
{
    internal class ProcessChangeEventRequest : IRequest
    {
        public ChangeStreamDocument<dynamic> Document { get; set; }
    }
}
