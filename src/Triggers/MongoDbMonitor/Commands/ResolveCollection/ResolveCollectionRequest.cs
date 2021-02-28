using MediatR;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.ResolveCollection
{
    internal class ResolveCollectionRequest : IRequest
    {
        public string CollectionName { get; set; }

        public string AssemblyName { get; internal set; }

        public string HandlerRequestFullQualifiedName { get; set; }

        public IDictionary<string, object> Values { get; set; }
    }
}
