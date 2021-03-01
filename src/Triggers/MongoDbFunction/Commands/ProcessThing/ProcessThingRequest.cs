using MongoDbMonitor.Commands.ExtractDocumentIdentifier;

namespace MongoDbFunction.Commands.ProcessThing
{
    public sealed class ProcessThingRequest : ExtractDocumentIdentifierRequest
    {
        public override string PropertyToExtract => "_id";
        public override string CollectionName => "things";
    }
}
