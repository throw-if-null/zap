using MediatR;
using MongoDbMonitor.Commands.Common.Responses;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.ProcessChangeEvent
{
    internal class ProcessChangeEventRequest : IRequest<ProcessingStatusResponse>
    {
        public string  CollectionName { get; set; }

        public string OperationName { get; set; }

        public IDictionary<string, object> Values { get; set; }
    }
}
