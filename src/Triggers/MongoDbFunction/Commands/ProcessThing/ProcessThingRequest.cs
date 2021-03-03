using MongoDbMonitor.Commands.ExtractDocumentIdentifier;

namespace MongoDbFunction.Commands.ProcessThing
{
    public sealed class ProcessThingRequest : ExtractDocumentIdentifierRequest
    {
        public override string PropertyToExtract => "SomeId";

        public override string CollectionName => "things";
    }
}
