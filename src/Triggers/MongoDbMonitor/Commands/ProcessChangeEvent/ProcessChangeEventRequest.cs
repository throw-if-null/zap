using MediatR;
using MongoDB.Driver;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.ProcessChangeEvent
{
    internal class ProcessChangeEventRequest : IRequest
    {
        public string  CollectionName { get; set; }

        public ChangeStreamOperationType OperationType { get; set; }

        public IDictionary<string, object> Values { get; set; }
    }
}
