using System.Collections.ObjectModel;

namespace MongoDbFunction
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public Collection<CollectionOptions> Collections { get; set; }

        public Collection<string> OperationTypes { get; set; }
    }

    public class CollectionOptions
    {
        public string Name { get; set; }

        public string HandlerNamespace { get; set; }
    }
}
