using MediatR;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.ExtractDocumentIdentifier
{
    public abstract class ExtractDocumentIdentifierRequest : IRequest
    {
        public IDictionary<string, object> Values { get; set; }

        public abstract string PropertyToExtract { get; }

        public abstract string CollectionName { get; }
    }
}
