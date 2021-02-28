using MediatR;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.ProcessDocument
{
    public abstract class ProcessDocumentRequest : IRequest
    {
        public string CollectionName { get; set; }

        public IDictionary<string, object> Values { get; set; }
    }
}
