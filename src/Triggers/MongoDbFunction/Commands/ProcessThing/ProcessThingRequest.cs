using MongoDbMonitor.Commands.ProcessDocument;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MongoDbFunction.Commands.ProcessThing
{
    public sealed class ProcessThingRequest : ProcessDocumentRequest
    {
        public override void OnError([NotNull] Exception ex)
        {
            return;
        }
    }
}
