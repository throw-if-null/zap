namespace MongoDbMonitor.Commands.Common.Responses
{
    public enum ProcessingStep
    {
        Unknown = 0,
        ProcessChangeEvent = 1,
        ResolveCollectionType = 2,
        ExtractDocumentIdentifier = 3,
        Notify = 4,
        SendSlackAlert = 5
    }
}
