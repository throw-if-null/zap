using MongoDbMonitor.Commands.ProcessDocument;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbFunction.Commands.ProcessItem
{
    public sealed class ProcessItemRequest : ProcessDocumentRequest
    {
        public override void OnError([NotNull] Exception ex)
        {
            return;
        }
    }
}
