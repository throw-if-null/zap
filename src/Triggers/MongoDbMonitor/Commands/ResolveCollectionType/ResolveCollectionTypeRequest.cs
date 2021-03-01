using MediatR;
using System.Collections.Generic;

namespace MongoDbMonitor.Commands.ResolveCollectionType
{
    internal class ResolveCollectionTypeRequest : IRequest
    {
        public string AssemblyName { get; internal set; }

        public string HandlerRequestFullQualifiedName { get; set; }

        public IDictionary<string, object> Values { get; set; }
    }
}
