using MediatR;
using MongoDbMonitor.Commands.Common.Responses;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.ExtractDocumentIdentifier
{
    public abstract class ExtractDocumentIdentifierRequest : IRequest<ProcessingStatusResponse>
    {
        public IDictionary<string, object> Values { get; set; }

        public abstract string PropertyToExtract { get; }

        public abstract string CollectionName { get; }
    }
}
