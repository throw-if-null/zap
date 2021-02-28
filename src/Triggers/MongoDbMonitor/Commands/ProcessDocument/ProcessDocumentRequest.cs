using MediatR;
using MongoDbMonitor.Commands.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbMonitor.Commands.ProcessDocument
{
    public abstract class ProcessDocumentRequest : IRequest, IOnRequestProcessingError
    {
        public string CollectionName { get; set; }

        public IDictionary<string, object> Values { get; set; }

        public abstract void OnError([NotNull] Exception ex);
    }
}
