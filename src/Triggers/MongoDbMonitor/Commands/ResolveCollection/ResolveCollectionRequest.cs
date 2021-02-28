using MediatR;
using MongoDbMonitor.Commands.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbMonitor.Commands.ResolveCollection
{
    internal class ResolveCollectionRequest : IRequest, IOnRequestProcessingError
    {
        public string CollectionName { get; set; }

        public string AssemblyName { get; internal set; }

        public string HandlerRequestFullQualifiedName { get; set; }

        public IDictionary<string, object> Values { get; set; }

        public void OnError([NotNull] Exception ex)
        {
            return;
        }
    }
}
