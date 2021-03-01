using MongoDbMonitor.Commands.ExtractDocumentIdentifier;

namespace MongoDbFunction.Commands.ProcessItem
{
    public sealed class ProcessItemRequest : ExtractDocumentIdentifierRequest
    {
        public override string PropertyToExtract => "_id";
        public override string CollectionName => "items";
    }
}
